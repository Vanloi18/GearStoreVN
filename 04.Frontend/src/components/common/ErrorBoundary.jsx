import React from 'react';

class ErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
        this.state = { hasError: false, error: null, errorInfo: null };
    }

    static getDerivedStateFromError(error) {
        // Update state so the next render will show the fallback UI.
        return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
        // You can also log the error to an error reporting service
        console.error("Uncaught error:", error, errorInfo);
        this.setState({ errorInfo });
    }

    render() {
        if (this.state.hasError) {
            return (
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    alignItems: 'center',
                    minHeight: '100vh',
                    backgroundColor: '#f8f9fa',
                    color: '#343a40',
                    fontFamily: 'system-ui, -apple-system, sans-serif'
                }}>
                    <h1 style={{ fontSize: '2rem', marginBottom: '1rem', color: '#dc3545' }}>Something went wrong.</h1>
                    <p style={{ maxWidth: '600px', textAlign: 'center', marginBottom: '2rem', lineHeight: '1.6' }}>
                        An unexpected error has occurred in the application. Please try reloading the page or return to the dashboard.
                    </p>
                    <details style={{ whiteSpace: 'pre-wrap', marginBottom: '2rem', padding: '1rem', background: '#e9ecef', borderRadius: '4px', maxWidth: '80%', overflow: 'auto' }}>
                        {this.state.error && this.state.error.toString()}
                    </details>
                    <div style={{ display: 'flex', gap: '1rem' }}>
                        <button
                            onClick={() => window.location.reload()}
                            style={{
                                padding: '0.75rem 1.5rem',
                                backgroundColor: '#6c757d',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                cursor: 'pointer',
                                fontSize: '1rem'
                            }}
                        >
                            Reload Page
                        </button>
                        <button
                            onClick={() => window.location.href = '/admin'}
                            style={{
                                padding: '0.75rem 1.5rem',
                                backgroundColor: '#007bff',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                cursor: 'pointer',
                                fontSize: '1rem'
                            }}
                        >
                            Back to Dashboard
                        </button>
                    </div>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
