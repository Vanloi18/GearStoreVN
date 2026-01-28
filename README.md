# GearStore - Enterprise E-Commerce Platform

[![CI/CD](https://github.com/yourusername/gearstore/actions/workflows/ci.yml/badge.svg)](https://github.com/yourusername/gearstore/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A production-ready, enterprise-grade e-commerce platform built with Clean Architecture principles, featuring a powerful admin panel, JWT authentication, and comprehensive testing.

## 🚀 Features

### Core Functionality
- ✅ Product catalog management (CRUD)
- ✅ Shopping cart system
- ✅ Order processing with status tracking
- ✅ User authentication & authorization (JWT)
- ✅ Role-based access control (Admin/User)
- ✅ Admin dashboard with analytics
- ✅ Soft delete for products
- ✅ Stock management with automatic updates

### Technical Features
- ✅ Clean Architecture (Domain, Application, Infrastructure, Presentation)
- ✅ RESTful API with Swagger documentation
- ✅ Entity Framework Core with MySQL
- ✅ Comprehensive unit & integration tests (xUnit)
- ✅ Docker containerization
- ✅ CI/CD with GitHub Actions
- ✅ Production-ready deployment configuration

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Client Layer                         │
│  ┌──────────────────┐              ┌──────────────────┐     │
│  │  Admin Frontend  │              │  Public Frontend │     │
│  │   (React/Vite)   │              │   (React/Vite)   │     │
│  └────────┬─────────┘              └────────┬─────────┘     │
└───────────┼────────────────────────────────┼───────────────┘
            │                                 │
            │         HTTPS (nginx)           │
            └────────────┬────────────────────┘
                         │
┌────────────────────────┼────────────────────────────────────┐
│                        │  Presentation Layer                 │
│              ┌─────────▼─────────┐                           │
│              │   ASP.NET Core    │                           │
│              │   Web API (.NET 9)│                           │
│              │   Controllers     │                           │
│              └─────────┬─────────┘                           │
└────────────────────────┼────────────────────────────────────┘
                         │
┌────────────────────────┼────────────────────────────────────┐
│                        │  Application Layer                  │
│              ┌─────────▼─────────┐                           │
│              │   Services        │                           │
│              │   DTOs            │                           │
│              │   Interfaces      │                           │
│              └─────────┬─────────┘                           │
└────────────────────────┼────────────────────────────────────┘
                         │
┌────────────────────────┼────────────────────────────────────┐
│                        │  Domain Layer                       │
│              ┌─────────▼─────────┐                           │
│              │   Entities        │                           │
│              │   Enums           │                           │
│              │   Value Objects   │                           │
│              └─────────┬─────────┘                           │
└────────────────────────┼────────────────────────────────────┘
                         │
┌────────────────────────┼────────────────────────────────────┐
│                        │  Infrastructure Layer               │
│              ┌─────────▼─────────┐                           │
│              │   EF Core DbContext│                          │
│              │   Repositories    │                           │
│              │   UnitOfWork      │                           │
│              └─────────┬─────────┘                           │
└────────────────────────┼────────────────────────────────────┘
                         │
                  ┌──────▼──────┐
                  │   MySQL 8   │
                  │   Database  │
                  └─────────────┘

Authentication Flow:
Client → Login API → JWT Token → Authenticated Requests → Role Check → API Response
```

## 🛠️ Tech Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Architecture**: Clean Architecture
- **Database**: MySQL 8.0 (Pomelo EF Core)
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT Bearer
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **Routing**: React Router DOM
- **HTTP Client**: Axios
- **Styling**: Plain CSS (no UI framework)

### DevOps
- **Containerization**: Docker, Docker Compose
- **CI/CD**: GitHub Actions
- **Web Server**: Nginx (reverse proxy)
- **SSL**: Let's Encrypt (Certbot)

### Testing
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Database**: EF Core InMemory

## 📁 Project Structure

```
GearStore/
├── 01.Core/
│   ├── GearStore.Domain/          # Entities, Enums, Value Objects
│   └── GearStore.Application/     # Services, DTOs, Interfaces
├── 02.Infrastructure/
│   └── GearStore.Infrastructure/  # EF Core, Repositories, Data Access
├── 03.Presentation/
│   └── GearStore.Web/             # API Controllers, Startup
├── 04.Frontend/                   # React Admin Panel
│   ├── src/
│   │   ├── api/                   # API clients
│   │   ├── components/            # Reusable components
│   │   ├── layouts/               # Layout components
│   │   ├── pages/                 # Page components
│   │   ├── routes/                # Route guards
│   │   └── utils/                 # Utility functions
│   ├── Dockerfile
│   └── nginx.conf
├── tests/
│   ├── GearStore.UnitTests/       # Unit tests
│   └── GearStore.IntegrationTests/# Integration tests
├── deployment/
│   └── nginx/                     # Nginx configurations
├── .github/
│   └── workflows/
│       └── ci.yml                 # GitHub Actions CI/CD
├── docker-compose.yml
├── Dockerfile
├── .env.example
├── DEPLOYMENT.md
└── README.md
```

## 🚦 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://www.docker.com/get-started) & [Docker Compose](https://docs.docker.com/compose/)
- [MySQL 8](https://dev.mysql.com/downloads/mysql/) (for local development without Docker)

### Quick Start with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/gearstore.git
   cd gearstore
   ```

2. **Configure environment**
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

3. **Start services**
   ```bash
   docker compose up -d
   ```

4. **Apply database migrations**
   ```bash
   docker exec -it gearstore-api dotnet ef database update
   ```

5. **Access the application**
   - **API**: http://localhost:5122
   - **Admin Panel**: http://localhost:3000
   - **Swagger**: http://localhost:5122/swagger (Development only)

6. **Default admin credentials**
   - Email: `admin@gearstore.com`
   - Password: `Admin@123`

### Local Development (Without Docker)

#### Backend

```bash
# Navigate to API project
cd 03.Presentation/GearStore.Web

# Restore dependencies
dotnet restore

# Update connection string in appsettings.Development.json
# Apply migrations
dotnet ef database update

# Run the API
dotnet run
```

#### Frontend

```bash
# Navigate to frontend
cd 04.Frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

## 🧪 Running Tests

### All Tests
```bash
dotnet test
```

### Unit Tests Only
```bash
dotnet test tests/GearStore.UnitTests/GearStore.UnitTests.csproj
```

### Integration Tests Only
```bash
dotnet test tests/GearStore.IntegrationTests/GearStore.IntegrationTests.csproj
```

### With Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📦 Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed production deployment instructions.

### Quick Deployment Summary

1. **Server Setup** (Ubuntu 22.04)
   ```bash
   sudo apt update && sudo apt upgrade -y
   sudo apt install -y docker.io docker-compose nginx certbot python3-certbot-nginx
   ```

2. **Clone & Configure**
   ```bash
   git clone https://github.com/yourusername/gearstore.git
   cd gearstore
   cp .env.example .env
   # Edit .env with production secrets
   ```

3. **Start Services**
   ```bash
   docker compose up -d
   docker exec -it gearstore-api dotnet ef database update
   ```

4. **Configure Nginx & SSL**
   ```bash
   sudo cp deployment/nginx/gearstore.conf /etc/nginx/sites-available/
   sudo ln -s /etc/nginx/sites-available/gearstore.conf /etc/nginx/sites-enabled/
   sudo certbot --nginx -d yourdomain.com
   ```

## 🔐 Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `MYSQL_ROOT_PASSWORD` | MySQL root password | `SecurePassword123!` |
| `MYSQL_DATABASE` | Database name | `GearStoreDb` |
| `JWT_SECRET` | JWT signing key (min 32 chars) | `your-super-secret-key-here` |
| `JWT_ISSUER` | JWT issuer | `GearStoreApi` |
| `JWT_AUDIENCE` | JWT audience | `GearStoreClient` |
| `JWT_EXPIRY_MINUTES` | Token expiration time | `60` |

## 📚 API Documentation

API documentation is available via Swagger UI in development mode:
- **URL**: http://localhost:5122/swagger
- **Note**: Disabled in production for security

### Key Endpoints

#### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/me` - Get current user info

#### Admin - Dashboard
- `GET /api/admin/dashboard` - Get dashboard statistics

#### Admin - Orders
- `GET /api/admin/orders` - List all orders
- `PUT /api/admin/orders/{id}/status` - Update order status

#### Admin - Users
- `GET /api/admin/users` - List all users
- `PUT /api/admin/users/{id}/role` - Update user role

#### Admin - Products
- `GET /api/admin/products` - List all products
- `DELETE /api/admin/products/{id}` - Soft delete product

## 🔒 Security Features

- ✅ JWT-based authentication
- ✅ Role-based authorization (Admin/User)
- ✅ Password hashing with ASP.NET Core Identity
- ✅ HTTPS enforcement in production
- ✅ Security headers (HSTS, X-Frame-Options, etc.)
- ✅ SQL injection protection (EF Core parameterized queries)
- ✅ CORS configuration
- ✅ Environment-based secrets management
- ⚠️ Rate limiting (recommended for production)
- ⚠️ CSRF protection (recommended for cookie-based auth)

## ⚡ Performance Optimizations

- ✅ AsNoTracking for read-only queries
- ✅ Async/await throughout
- ✅ Connection pooling
- ✅ Gzip compression (nginx)
- ✅ Docker multi-stage builds
- ✅ AsSplitQuery for complex includes
- ⚠️ Response caching (implement for static data)
- ⚠️ Redis caching (recommended for high traffic)

## 📊 Testing Coverage

- **Unit Tests**: 18 tests covering core business logic
- **Integration Tests**: 17 tests covering API endpoints
- **Total**: 35 tests

### Test Categories
- Order service (creation, status transitions, stock management)
- Product service (soft delete, filtering, search)
- User service (role management)
- Authentication & authorization
- Admin API endpoints

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- LinkedIn: [Your LinkedIn](https://linkedin.com/in/yourprofile)
- Email: your.email@example.com

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- ASP.NET Core team for the excellent framework
- React team for the frontend library
- All open-source contributors

## 📞 Support

For issues and questions:
- Open a [GitHub Issue](https://github.com/yourusername/gearstore/issues)
- Check [DEPLOYMENT.md](DEPLOYMENT.md) for deployment help
- Review existing issues before creating new ones

## 🗺️ Roadmap

- [ ] Implement refresh token rotation
- [ ] Add rate limiting middleware
- [ ] Implement Redis caching
- [ ] Add email notifications
- [ ] Implement payment gateway integration
- [ ] Add product reviews and ratings
- [ ] Implement advanced search with Elasticsearch
- [ ] Add real-time notifications (SignalR)
- [ ] Implement multi-language support
- [ ] Add comprehensive admin analytics dashboard

---

**Built with ❤️ using Clean Architecture and modern best practices**
