# GitHub Actions CI/CD Setup

This document explains how to set up GitHub Actions for automated CI/CD deployment of the Offroad Vehicle Rentals application.

## Workflows

### 1. CI - Build and Test (`ci.yml`)
- **Triggers**: Push or PR to `main` or `develop` branches
- **Purpose**: Builds all projects and runs tests
- **Artifacts**: Uploads build outputs for API and Web app

### 2. CD - Deploy API (`deploy-api.yml`)
- **Triggers**:
  - Push to `main` branch (only when API or Shared code changes)
  - Manual workflow dispatch
- **Purpose**: Deploys Azure Functions API to Azure
- **Target**: `offroad-vehicle-api` Function App

### 3. CD - Deploy Web App (`deploy-web.yml`)
- **Triggers**:
  - Push to `main` branch (only when Web or Shared code changes)
  - Manual workflow dispatch
- **Purpose**: Deploys Blazor Web App to Azure
- **Target**: `offroad-vehicle-web` App Service

## Required GitHub Secrets

You need to configure the following secret in your GitHub repository:

### `AZURE_CREDENTIALS`

This contains the service principal credentials for authenticating to Azure.

**Value** (JSON format - use the ENTIRE output from the service principal creation):

Run this command to get your credentials:
```bash
az ad sp create-for-rbac --name "offroad-vehicle-github-actions" --role contributor --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/brian_app --sdk-auth
```

Then copy the entire JSON output and paste it as the secret value.

## Setup Instructions

### Step 1: Add GitHub Secret

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_CREDENTIALS`
5. Value: Paste the entire JSON output from above
6. Click **Add secret**

### Step 2: Initialize Git Repository (if not already done)

```bash
cd /Users/brianhinterlong/RiderProjects/OffroadVehicleRentals
git init
git add .
git commit -m "Initial commit with GitHub Actions CI/CD"
```

### Step 3: Connect to GitHub

```bash
# Create a new repository on GitHub first, then:
git remote add origin https://github.com/YOUR_USERNAME/offroad-vehicle-rentals.git
git branch -M main
git push -u origin main
```

### Step 4: Verify Workflows

1. After pushing, go to your repository on GitHub
2. Click on the **Actions** tab
3. You should see the workflows running automatically

## Manual Deployment

You can manually trigger deployments:

1. Go to **Actions** tab in GitHub
2. Select either **CD - Deploy API to Azure Functions** or **CD - Deploy Web App to Azure**
3. Click **Run workflow**
4. Select the `main` branch
5. Click **Run workflow**

## Service Principal Details

- **Name**: `offroad-vehicle-github-actions`
- **Role**: Contributor
- **Scope**: Resource Group `brian_app`
- **Subscription**: `fe2945be-a2d6-4062-adef-361e1c6cd106`

## Workflow Features

### Smart Path Filtering
- API workflow only runs when API or Shared code changes
- Web workflow only runs when Web or Shared code changes
- This saves build minutes and speeds up deployments

### Artifacts
- CI workflow uploads build artifacts that can be downloaded for inspection
- Artifacts are retained for 1 day

### Security
- Azure credentials are stored as encrypted secrets
- Workflows log out from Azure after deployment
- Service principal has minimal required permissions (Contributor on resource group only)

## Troubleshooting

### Workflow fails with authentication error
- Verify `AZURE_CREDENTIALS` secret is correctly formatted JSON
- Check service principal hasn't expired
- Ensure service principal has Contributor role on the resource group

### Deployment succeeds but app doesn't work
- Check Azure App Service logs
- Verify environment variables and connection strings in Azure portal
- Ensure database connection string is configured in Azure

### Want to deploy to different environment (staging/production)
- Duplicate the deploy workflows
- Change the `AZURE_FUNCTIONAPP_NAME` and `AZURE_WEBAPP_NAME` environment variables
- Add branch filtering (e.g., `develop` for staging, `main` for production)
- Create separate service principals per environment for better security

## Security Best Practices

1. **Rotate credentials regularly**: Create a new service principal every 90 days
2. **Use separate service principals per environment**: Don't reuse production credentials for staging
3. **Enable branch protection**: Require PR reviews before merging to `main`
4. **Monitor deployments**: Set up Azure Monitor alerts for deployment failures

## Next Steps

- [ ] Set up branch protection rules on `main` branch
- [ ] Configure deployment slots for zero-downtime deployments
- [ ] Add integration tests to CI workflow
- [ ] Set up deployment notifications (Slack, email, etc.)
- [ ] Configure Azure Application Insights for monitoring
