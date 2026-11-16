#!/bin/bash

# Log output so the window doesn't close on you
exec > >(tee -a globomantics-webapp.log) 2>&1

echo "===== Script started at $(date) ====="

# ---- Config ----
RESOURCE_GROUP="rg-globomantics-mi-demo"
LOCATION="southcentralus"
APP_SERVICE_PLAN="plan-globomantics-demo"
WEBAPP_NAME="app-globomantics-$(openssl rand -hex 3)"   # must be globally unique
RUNTIME="DOTNET:8.0"                                   # change to DOTNET:6.0 if needed

echo "Creating resource group: $RESOURCE_GROUP"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

echo "Creating App Service Plan: $APP_SERVICE_PLAN"
az appservice plan create \
  --name "$APP_SERVICE_PLAN" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku B1 \
  --is-linux

echo "Creating Web App: $WEBAPP_NAME"
az webapp create \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$APP_SERVICE_PLAN" \
  --runtime "$RUNTIME"

echo "Enabling System-assigned Managed Identity"
az webapp identity assign \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP"

echo ""
echo "=========================================="
echo "Web App created successfully!"
echo "Name          : $WEBAPP_NAME"
echo "URL           : https://$WEBAPP_NAME.azurewebsites.net"
echo "Resource Group: $RESOURCE_GROUP"
echo "=========================================="
echo ""
echo "Use this name when you create the SQL user:"
echo "CREATE USER [$WEBAPP_NAME] FROM EXTERNAL PROVIDER;"
echo "ALTER ROLE db_owner ADD MEMBER [$WEBAPP_NAME];"

echo ""
echo "===== Script finished. Log saved to globomantics-webapp.log ====="
read -p "Press Enter to close..."