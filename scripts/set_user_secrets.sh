#!/usr/bin/env bash
# Helper script to initialize and set user-secrets for Bagman.Api (local dev).
# Run from repository root: bash scripts/set_user_secrets.sh

set -euo pipefail

API_DIR="src/Bagman.Api"
cd "$API_DIR"

if ! dotnet user-secrets list > /dev/null 2>&1; then
  echo "Initializing user-secrets for Bagman.Api..."
  dotnet user-secrets init
fi

echo "Setting example secrets (replace values)..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=BagmanDb;Trusted_Connection=True;MultipleActiveResultSets=true"

dotnet user-secrets set "Jwt:Secret" "replace-with-a-long-random-secret-at-least-32-chars"

dotnet user-secrets set "Jwt:AccessTokenExpirationMinutes" "60"

dotnet user-secrets set "Jwt:RefreshTokenExpirationDays" "7"

echo "User-secrets configured for Bagman.Api"
