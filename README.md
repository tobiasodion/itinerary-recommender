# itinerary-recommender
A cloud-based system that leverages GPT completions API to recommend tour itinerary for a given city

## To run locally (VS Code)

### Requirements
- Azure account + subscription
- Azure Tools for VSCode
- .Net 6

### Steps
1. Setup Azure function on the Azure account using the Azure tools
2. Create a local configuration file `local.settings.json`
3. Paste the following configuration and update placeholders with values from your cloud Azure function

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=<your-account-name>;AccountKey=<your-storage-account-key>;EndpointSuffix=core.windows.net",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING": "DefaultEndpointsProtocol=https;AccountName=<your-account-name>;AccountKey=<your-storage-account-key>;EndpointSuffix=core.windows.net",
    "WEBSITE_CONTENTSHARE": "<your-account-value>",
    "WEBSITE_RUN_FROM_PACKAGE": "1",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "<your-instrumentation-key>"
  }
}
```
4. Start Debugging
