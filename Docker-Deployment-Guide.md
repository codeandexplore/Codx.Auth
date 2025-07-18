# Docker Deployment Guide for Codx.Auth IdentityServer with NGINX

## Problem Solved
This configuration fixes the SSL certificate error and login loop issues when running the IdentityServer application in production Docker containers behind an NGINX reverse proxy.

## Key Changes Made

### 1. Cookie Configuration for Docker
- Added proper SameSite and secure policy settings
- Configured cookie policies for container environments
- Added essential cookie marking for authentication

### 2. Data Protection Configuration
- Added persistent data protection keys to `/app/keys` directory
- Ensures authentication cookies remain valid across container restarts
- Created directory with proper permissions in Dockerfile

### 3. HTTPS/SSL Configuration
- Disabled HTTPS redirection in production environments
- Configured application to run HTTP-only in containers
- Added proper forwarded headers configuration for NGINX reverse proxy

### 4. Dockerfile Updates
- Removed HTTPS port exposure (8081) for production
- Added data protection keys directory creation
- Set proper permissions for the keys directory
- Fixed malformed ENTRYPOINT syntax

## Configuration Files

### appsettings.json (Base configuration)
- Removed HTTPS endpoint for production
- Added IdentityServer RequireHttpsMetadata: false

### appsettings.Production.json (Production-specific)
- HTTP-only Kestrel configuration
- IdentityServer HTTPS metadata disabled
- Enhanced logging for troubleshooting
- Specific forwarded headers configuration for NGINX

### appsettings.Development.json (Development)
- Keeps HTTPS for local development
- Maintains development SSL capabilities

## Docker Deployment

### 1. Build the Docker imagedocker build -t codx-auth .
### 2. Run behind NGINX (Recommended)docker run -d \
  --name codx-auth \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -v /app/data/keys:/app/keys \
  codx-auth
### 3. Docker Compose Exampleversion: '3.8'

services:
  codx-auth:
    build: .
    container_name: codx-auth
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    ports:
      - "127.0.0.1:8080:8080"
    volumes:
      - ./data/keys:/app/keys
    restart: unless-stopped
    
  nginx:
    image: nginx:alpine
    container_name: nginx-proxy
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./ssl:/etc/nginx/ssl
    depends_on:
      - codx-auth
    restart: unless-stopped
## NGINX Configuration for IdentityServer

### Complete nginx.confevents {
    worker_connections 1024;
}

http {
    upstream codx-auth {
        server codx-auth:8080;
        # If running on host network: server 127.0.0.1:8080;
    }

    # Redirect HTTP to HTTPS
    server {
        listen 80;
        server_name your-domain.com;
        return 301 https://$server_name$request_uri;
    }

    # HTTPS server for IdentityServer
    server {
        listen 443 ssl http2;
        server_name your-domain.com;

        # SSL Configuration
        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
        ssl_prefer_server_ciphers off;

        # Security Headers
        add_header X-Frame-Options DENY;
        add_header X-Content-Type-Options nosniff;
        add_header X-XSS-Protection "1; mode=block";
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;

        # Proxy Configuration for IdentityServer
        location / {
            proxy_pass http://codx-auth;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_set_header X-Forwarded-Host $host;
            proxy_set_header X-Forwarded-Port $server_port;
            
            # Important for IdentityServer
            proxy_cache_bypass $http_upgrade;
            proxy_buffering off;
            proxy_read_timeout 100s;
            proxy_connect_timeout 10s;
            proxy_send_timeout 100s;
        }

        # Specific configuration for IdentityServer discovery endpoint
        location /.well-known/openid_configuration {
            proxy_pass http://codx-auth;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_set_header X-Forwarded-Host $host;
            
            # Cache the discovery document
            proxy_cache_valid 200 1h;
            add_header Cache-Control "public, max-age=3600";
        }
    }
}
### Simple NGINX site configuration (if using sites-available)server {
    listen 443 ssl http2;
    server_name your-identityserver-domain.com;

    ssl_certificate /path/to/your/cert.pem;
    ssl_certificate_key /path/to/your/key.pem;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $host;
        proxy_set_header X-Forwarded-Port $server_port;
        
        # Important for authentication cookies
        proxy_set_header Connection keep-alive;
        proxy_cache_bypass $http_upgrade;
        proxy_buffering off;
    }
}
## Environment Variables for ProductionASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
## Troubleshooting

### If login still loops:
1. Verify NGINX is setting all required headers:curl -I https://your-domain.com/.well-known/openid_configuration2. Check that cookies are being set correctly in browser dev tools
3. Verify data protection keys are persisting: `docker exec codx-auth ls -la /app/keys`
4. Check container logs: `docker logs codx-auth`

### If certificate errors persist:
1. Ensure ASPNETCORE_ENVIRONMENT=Production is set
2. Verify NGINX SSL configuration is correct
3. Test internal container connectivity: `docker exec nginx-proxy curl http://codx-auth:8080/.well-known/openid_configuration`

### NGINX Testing Commands:# Test NGINX configuration
docker exec nginx-proxy nginx -t

# Reload NGINX configuration
docker exec nginx-proxy nginx -s reload

# Check NGINX logs
docker logs nginx-proxy
## Security Notes
- Container runs HTTP internally (port 8080) - this is secure
- NGINX handles all SSL/TLS termination
- Data protection keys are persisted to prevent authentication issues
- All external traffic is HTTPS via NGINX
- Internal Docker network traffic is HTTP (secure within Docker network)

## Production Checklist
- [ ] SSL certificate is valid and properly configured in NGINX
- [ ] All required NGINX headers are being forwarded
- [ ] Data protection keys directory is persistent
- [ ] Container is running with Production environment
- [ ] NGINX configuration tested and validated
- [ ] Logs are being monitored for both containers