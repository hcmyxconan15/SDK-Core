# Installing `com.undercats.sdkcore` via OpenUPM

This guide explains how to install the `com.undercats.sdkcore` package into your Unity project using **Unity Package Manager (UPM)** and **OpenUPM scoped registry**.


## Prerequisites
- Unity version **2019.4 LTS** or newer is recommended.
- Internet connection (to fetch package from OpenUPM).


## Installation Steps

### 1. Add OpenUPM Scoped Registry
1. Open your Unity project.
2. Go to **Edit → Project Settings → Package Manager**.
3. Under **Scoped Registries**, click the **+** button.
4. Enter the following details:
   - **Name:** `OpenUPM`
   - **URL:** `https://package.openupm.com`
   - **Scopes:** `com.undercats`
5. Save the settings.

### 2. Add Package via Unity Package Manager
1. Open **Window → Package Manager**.
2. In the top-left corner, click **+** and select **Add by Name...**.
3. Enter:
   - **Name:** `com.undercats.sdkcore`
   - **Version:** (optional, leave blank for latest version)
4. Click **Add**.


### 3. Verify Installation
- Once added, you will see **UnderCats SDK Core** in the list of installed packages in Package Manager.
- You can now start using the SDK in your project.


## Alternative: Add Manually to `manifest.json`
If you prefer, you can add the package manually:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": ["com.undercats"]
    }
  ],
  "dependencies": {
    "com.undercats.sdkcore": "*"
  }
}
