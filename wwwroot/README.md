# Node.js Project Setup and Build Instructions

## Installation

### 1. Install Node Version Manager (nvm)
To manage multiple versions of Node.js, install `nvm` by following the instructions from the [nvm GitHub repository](https://github.com/nvm-sh/nvm?tab=readme-ov-file#installing-and-updating).

### 2. Install Node.js
Once `nvm` is installed, you can install the latest version of Node.js:

```bash
nvm install node
```
After installation, set the newly installed version as the active version:
```bash
nvm use node
```

# Project Setup

## 1. Install Dependencies
```bash
npm install
```

## 2. Build the Project
```bash
npx vite build
```

# Debugging with VSCode and Chrome
## 1. Create a VSCode Debug Configuration
To debug your Node.js application with VSCode and Chrome, you need to create a debug configuration in your `.vscode/launch.json` file. If the file doesn't exist, create it in the `.vscode` directory at the root of your project.

Add the following configuration to your `launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "chrome",
      "request": "launch",
      "name": "Launch Chrome against localhost",
      "url": "http://localhost:3000",
      "webRoot": "${workspaceFolder}"
    }
  ]
}
```
## 2. Run the Development Server
Start your development server by running:

```bash
npm run dev
```
### 3. Start Debugging
- Open the Debug view in VSCode by clicking on the Debug icon in the Activity Bar on the side of the window.

- Select the "Launch Chrome against localhost" configuration from the dropdown.

- Click the green play button to start debugging.

This will launch Chrome and attach the VSCode debugger to it. You can set breakpoints in your code, and VSCode will pause execution at those points, allowing you to inspect variables and step through your code.

