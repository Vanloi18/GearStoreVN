# GearStore Production Deployment Guide

## Prerequisites

- Ubuntu 22.04 LTS server
- Root or sudo access
- Domain name pointed to server IP
- Minimum 2GB RAM, 2 CPU cores, 20GB disk

## Step 1: Initial Server Setup

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install required packages
sudo apt install -y curl git ufw

# Configure firewall
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
sudo ufw status
```

## Step 2: Install Docker & Docker Compose

```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
newgrp docker

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker --version
docker-compose --version
```

## Step 3: Clone Repository

```bash
cd /opt
sudo git clone https://github.com/yourusername/gearstore.git
sudo chown -R $USER:$USER gearstore
cd gearstore
```

## Step 4: Configure Environment

```bash
# Copy environment template
cp .env.example .env

# Edit environment variables
nano .env
```

Set the following values:
```env
MYSQL_ROOT_PASSWORD=<strong-random-password>
MYSQL_DATABASE=GearStoreDb
JWT_SECRET=<min-64-character-random-string>
JWT_ISSUER=GearStoreApi
JWT_AUDIENCE=GearStoreClient
JWT_EXPIRY_MINUTES=60
```

Generate secure secrets:
```bash
# Generate MySQL password
openssl rand -base64 32

# Generate JWT secret
openssl rand -base64 64
```

## Step 5: Start Services

```bash
# Start all services
docker compose up -d

# Check status
docker compose ps

# View logs
docker compose logs -f
```

## Step 6: Apply Database Migrations

```bash
# Wait for MySQL to be ready (30 seconds)
sleep 30

# Apply migrations
docker exec -it gearstore-api dotnet ef database update

# Verify database
docker exec -it gearstore-mysql mysql -u root -p -e "SHOW DATABASES;"
```

## Step 7: Install and Configure Nginx

```bash
# Install nginx
sudo apt install -y nginx

# Remove default config
sudo rm /etc/nginx/sites-enabled/default

# Copy GearStore nginx config
sudo cp deployment/nginx/gearstore.conf /etc/nginx/sites-available/gearstore

# Edit domain name
sudo nano /etc/nginx/sites-available/gearstore
# Replace yourdomain.com with your actual domain

# Enable site
sudo ln -s /etc/nginx/sites-available/gearstore /etc/nginx/sites-enabled/

# Test configuration
sudo nginx -t

# Restart nginx
sudo systemctl restart nginx
sudo systemctl enable nginx
```

## Step 8: Setup SSL with Let's Encrypt

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obtain SSL certificate
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com

# Follow prompts:
# - Enter email address
# - Agree to terms
# - Choose redirect HTTP to HTTPS (option 2)

# Verify auto-renewal
sudo certbot renew --dry-run

# Auto-renewal is configured via systemd timer
sudo systemctl status certbot.timer
```

## Step 9: Verify Deployment

```bash
# Check all services are running
docker compose ps

# Check nginx status
sudo systemctl status nginx

# Test API endpoint
curl https://yourdomain.com/api/health

# Test admin frontend
curl https://yourdomain.com

# View application logs
docker compose logs api
docker compose logs admin-frontend
docker compose logs mysql
```

## Step 10: Configure Automatic Backups

```bash
# Create backup directory
sudo mkdir -p /opt/backups/gearstore

# Create backup script
sudo nano /opt/backups/backup-gearstore.sh
```

Backup script content:
```bash
#!/bin/bash
BACKUP_DIR="/opt/backups/gearstore"
DATE=$(date +%Y%m%d_%H%M%S)
MYSQL_CONTAINER="gearstore-mysql"
MYSQL_PASSWORD="your_mysql_root_password"

# Backup database
docker exec $MYSQL_CONTAINER mysqldump -u root -p$MYSQL_PASSWORD GearStoreDb > $BACKUP_DIR/db_backup_$DATE.sql

# Compress backup
gzip $BACKUP_DIR/db_backup_$DATE.sql

# Delete backups older than 7 days
find $BACKUP_DIR -name "*.sql.gz" -mtime +7 -delete

echo "Backup completed: $DATE"
```

Make executable and schedule:
```bash
# Make executable
sudo chmod +x /opt/backups/backup-gearstore.sh

# Add to crontab (daily at 2 AM)
sudo crontab -e
# Add line:
0 2 * * * /opt/backups/backup-gearstore.sh >> /var/log/gearstore-backup.log 2>&1
```

## Step 11: Setup Monitoring

```bash
# Install monitoring tools
sudo apt install -y htop nethogs

# Monitor Docker containers
docker stats

# Monitor logs in real-time
docker compose logs -f --tail=100

# Check disk usage
df -h
docker system df
```

## Step 12: Security Hardening

```bash
# Disable root SSH login
sudo nano /etc/ssh/sshd_config
# Set: PermitRootLogin no
sudo systemctl restart sshd

# Install fail2ban
sudo apt install -y fail2ban
sudo systemctl enable fail2ban
sudo systemctl start fail2ban

# Configure automatic security updates
sudo apt install -y unattended-upgrades
sudo dpkg-reconfigure -plow unattended-upgrades
```

## Maintenance Commands

### Update Application
```bash
cd /opt/gearstore
git pull origin main
docker compose down
docker compose build
docker compose up -d
docker exec -it gearstore-api dotnet ef database update
```

### Restart Services
```bash
# Restart all
docker compose restart

# Restart specific service
docker compose restart api
docker compose restart mysql
docker compose restart admin-frontend
```

### View Logs
```bash
# All logs
docker compose logs -f

# Specific service
docker compose logs -f api

# Last 100 lines
docker compose logs --tail=100 api
```

### Database Backup (Manual)
```bash
docker exec gearstore-mysql mysqldump -u root -p GearStoreDb > backup_$(date +%Y%m%d).sql
```

### Database Restore
```bash
docker exec -i gearstore-mysql mysql -u root -p GearStoreDb < backup_20260128.sql
```

### Clean Docker Resources
```bash
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Full cleanup
docker system prune -a --volumes
```

## Troubleshooting

### API not responding
```bash
docker compose logs api
docker compose restart api
```

### Database connection errors
```bash
docker compose logs mysql
docker compose restart mysql
# Wait 30 seconds
docker exec -it gearstore-api dotnet ef database update
```

### Nginx errors
```bash
sudo nginx -t
sudo systemctl status nginx
sudo tail -f /var/log/nginx/error.log
```

### SSL certificate issues
```bash
sudo certbot certificates
sudo certbot renew --force-renewal
sudo systemctl restart nginx
```

### Out of disk space
```bash
df -h
docker system df
docker system prune -a --volumes
sudo journalctl --vacuum-time=3d
```

## Health Checks

```bash
# API health
curl https://yourdomain.com/api/health

# Database health
docker exec gearstore-mysql mysqladmin ping -h localhost -u root -p

# Container health
docker compose ps

# System resources
htop
docker stats
```

## Default Credentials

**Admin Account:**
- Email: admin@gearstore.com
- Password: Admin@123

**⚠️ CRITICAL: Change admin password immediately after first login!**

## Post-Deployment Checklist

- [ ] All services running (docker compose ps)
- [ ] Database migrations applied
- [ ] SSL certificate installed and auto-renewal configured
- [ ] Firewall configured (ports 80, 443, 22 only)
- [ ] Admin password changed
- [ ] Backup cron job configured
- [ ] Monitoring tools installed
- [ ] Security hardening completed
- [ ] Application accessible via HTTPS
- [ ] API endpoints responding correctly
- [ ] Admin panel accessible and functional

## Support

For issues, check logs first:
```bash
docker compose logs -f
sudo tail -f /var/log/nginx/error.log
```

## Rollback Procedure

```bash
cd /opt/gearstore
git log --oneline
git checkout <previous-commit-hash>
docker compose down
docker compose up -d --build
```
