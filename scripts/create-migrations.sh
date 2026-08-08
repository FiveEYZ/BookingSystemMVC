#!/usr/bin/env bash

# Script to create EF Core migrations for both Identity and Booking DbContexts.
# Run this locally where dotnet-ef and dotnet
# are available. This script does not run on the server/CI — it's a helper.

set -e

echo "Creating migrations..."

# Identity migrations
dotnet ef migrations add InitialIdentity --context ApplicationDbContext -o Data/Migrations/Identity

# Booking migrations
dotnet ef migrations add InitialBooking --context BookingDbContext -o Booking/Data/Migrations/Booking

echo "Migrations created. Run 'dotnet ef database update --context ApplicationDbContext' and 'dotnet ef database update --context BookingDbContext' to apply them."
