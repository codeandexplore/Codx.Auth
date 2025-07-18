#!/bin/bash

# Codx.Auth IdentityServer Deployment Script
# This script helps deploy the IdentityServer application with NGINX

set -e

echo "?? Deploying Codx.Auth IdentityServer with NGINX..."

# Create necessary directories
echo "?? Creating directories..."
mkdir -p data/keys
mkdir -p nginx/ssl
mkdir -p logs

# Set proper permissions for data protection keys
chmod 755 data/keys

# Check if SSL certificates exist
if [ ! -f "nginx/ssl/cert.pem" ] || [ ! -f "nginx/ssl/key.pem" ]; then
    echo "??  SSL certificates not found!"
    echo "Please place your SSL certificates in nginx/ssl/ directory:"
    echo "  - nginx/ssl/cert.pem"
    echo "  - nginx/ssl/key.pem"
    echo ""
    echo "For testing, you can generate self-signed certificates:"
    echo "  openssl req -x509 -nodes -days 365 -newkey rsa:2048 \\"
    echo "    -keyout nginx/ssl/key.pem \\"
    echo "    -out nginx/ssl/cert.pem \\"
    echo "    -subj '/CN=localhost'"
    echo ""
    read -p "Continue anyway? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Build the Docker image
echo "?? Building Docker image..."
docker build -t codx-auth -f src/Codx.Auth/Dockerfile .

# Stop existing containers if running
echo "?? Stopping existing containers..."
docker-compose down 2>/dev/null || true

# Start the services
echo "?? Starting services..."
docker-compose up -d

# Wait for services to start
echo "? Waiting for services to start..."
sleep 10

# Check if services are running
echo "? Checking service status..."
if docker-compose ps | grep -q "Up"; then
    echo "?? Services are running!"
    echo ""
    echo "?? Service Status:"
    docker-compose ps
    echo ""
    echo "?? Your IdentityServer should be available at:"
    echo "  - HTTP:  http://localhost (redirects to HTTPS)"
    echo "  - HTTPS: https://localhost"
    echo "  - Discovery: https://localhost/.well-known/openid_configuration"
    echo ""
    echo "?? To view logs:"
    echo "  - IdentityServer: docker logs codx-auth"
    echo "  - NGINX: docker logs nginx-proxy"
    echo "  - All services: docker-compose logs -f"
    echo ""
    echo "???  To stop services: docker-compose down"
else
    echo "? Some services failed to start. Check logs:"
    docker-compose logs
    exit 1
fi