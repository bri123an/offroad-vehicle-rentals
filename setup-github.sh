#!/bin/bash

echo "=========================================="
echo "GitHub Repository Setup Script"
echo "=========================================="
echo ""
echo "This script will help you create and push to GitHub"
echo ""

# Check if we're in a git repo
if [ ! -d .git ]; then
    echo "❌ Error: Not a git repository"
    exit 1
fi

echo "✅ Git repository detected"
echo ""

# Prompt for GitHub username
read -p "Enter your GitHub username: " GITHUB_USERNAME

if [ -z "$GITHUB_USERNAME" ]; then
    echo "❌ Error: GitHub username is required"
    exit 1
fi

REPO_NAME="offroad-vehicle-rentals"
REPO_URL="https://github.com/$GITHUB_USERNAME/$REPO_NAME.git"

echo ""
echo "Repository will be created at:"
echo "  $REPO_URL"
echo ""
echo "=========================================="
echo "STEP 1: Create the GitHub Repository"
echo "=========================================="
echo ""
echo "Please go to: https://github.com/new"
echo ""
echo "And create a repository with these settings:"
echo "  - Repository name: $REPO_NAME"
echo "  - Description: Offroad vehicle rental management system"
echo "  - Public or Private: Your choice"
echo "  - ⚠️  DO NOT initialize with README, .gitignore, or license"
echo ""
read -p "Press ENTER once you've created the repository..."

echo ""
echo "=========================================="
echo "STEP 2: Adding Remote and Pushing Code"
echo "=========================================="
echo ""

# Check if remote already exists
if git remote | grep -q "^origin$"; then
    echo "⚠️  Remote 'origin' already exists. Removing it..."
    git remote remove origin
fi

# Add remote
echo "Adding remote origin..."
git remote add origin "$REPO_URL"

if [ $? -ne 0 ]; then
    echo "❌ Error: Failed to add remote"
    exit 1
fi

echo "✅ Remote added successfully"
echo ""

# Rename branch to main if needed
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo "Renaming branch to 'main'..."
    git branch -M main
fi

echo "Pushing to GitHub..."
git push -u origin main

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Error: Failed to push to GitHub"
    echo ""
    echo "This might be due to:"
    echo "  1. Repository doesn't exist on GitHub"
    echo "  2. Authentication issues (you may need to enter credentials)"
    echo "  3. Repository URL is incorrect"
    echo ""
    echo "Try running manually:"
    echo "  git push -u origin main"
    exit 1
fi

echo ""
echo "✅ Code pushed successfully!"
echo ""

echo "=========================================="
echo "STEP 3: Add AZURE_CREDENTIALS Secret"
echo "=========================================="
echo ""
echo "Go to: https://github.com/$GITHUB_USERNAME/$REPO_NAME/settings/secrets/actions"
echo ""
echo "Click 'New repository secret' and add:"
echo "  Name: AZURE_CREDENTIALS"
echo "  Value: Copy from .github/ADD_SECRET_INSTRUCTIONS.md"
echo ""
echo "Full instructions: .github/ADD_SECRET_INSTRUCTIONS.md"
echo ""
read -p "Press ENTER once you've added the secret..."

echo ""
echo "=========================================="
echo "STEP 4: Verify Workflows"
echo "=========================================="
echo ""
echo "Go to: https://github.com/$GITHUB_USERNAME/$REPO_NAME/actions"
echo ""
echo "You should see:"
echo "  ✅ CI - Build and Test (running or completed)"
echo "  ✅ CD workflows ready to run on next push"
echo ""
echo "🎉 Setup complete!"
echo ""
echo "Next steps:"
echo "  - Check Actions tab to see CI workflow running"
echo "  - Make a change and push to trigger deployment"
echo "  - View your app at: https://offroad-vehicle-web.azurewebsites.net"
echo ""
