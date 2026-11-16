#!/bin/bash

# Log to file and console
exec > >(tee -a globomantics-sql.log) 2>&1

echo "===== Script started at $(date) ====="

# ---- Config ----
RESOURCE_GROUP="rg-globomantics-mi-demo"
LOCATION="westus2"
ADMIN_USER="globomanticsadmin"
ADMIN_PASSWORD="P@ssw0rd-Globomantics-2026!"
DATABASE_NAME="GlobomanticsDb"

RANDOM_SUFFIX=$(openssl rand -hex 3)
SQL_SERVER_NAME="sql-globomantics-${RANDOM_SUFFIX}"

echo "Creating resource group: $RESOURCE_GROUP"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

echo "Creating SQL Server: $SQL_SERVER_NAME"
az sql server create \
  --name "$SQL_SERVER_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --admin-user "$ADMIN_USER" \
  --admin-password "$ADMIN_PASSWORD"

echo "Creating database: $DATABASE_NAME"
az sql db create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER_NAME" \
  --name "$DATABASE_NAME" \
  --service-objective Basic

echo "Adding firewall rule to allow Azure services"
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER_NAME" \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

echo ""
echo "=========================================="
echo "Server created successfully!"
echo "Server name : $SQL_SERVER_NAME.database.windows.net"
echo "Database    : $DATABASE_NAME"
echo "=========================================="
echo ""
echo "Connection string:"
echo "Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Database=${DATABASE_NAME};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo ""
echo "===== Script finished. Full log saved to globomantics-sql.log ====="
read -p "Press Enter to close..."