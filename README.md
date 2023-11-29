# itinerary-recommender
A cloud-based system that leverages GPT completions API to recommend tour itineraries for a given city

# 1. To Debug locally (VS Code)

### Requirements
- Azure account + subscription
- Azure Tools for VSCode
- .Net 6

### Steps
1. Clone this repository to the local environment
1. Setup Azure resources on Azure cloud using the Azure tools [see link](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=node-v3%2Cpython-v2%2Cisolated-process&pivots=programming-language-csharp#create-an-azure-functions-project). You must be signed in [see link](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=node-v3%2Cpython-v2%2Cisolated-process&pivots=programming-language-csharp#sign-in-to-azure)
2. Deploy the Azure Function App using the Azure tools [see link](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=node-v3%2Cpython-v2%2Cisolated-process&pivots=programming-language-csharp#republish-project-files)
3. Download the function app configuration settings to your local project. [see link](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=node-v3%2Cpython-v2%2Cisolated-process&pivots=programming-language-csharp#download-settings-from-azure)
4. The downloaded `local.settings.json` file will contain some settings. Update the file with the following configurations - `GPT_API_KEY`, `GPT_API_BASE_URL`, `GPT_API_COMPLETION_ENDPOINT`, `EMAIL_ACCOUNT`, `EMAIL_PASSWORD`, `SMTP_HOST`, `SMTP_PORT`.
5. The `local.settings.json` file should be similar to the following template after the update.

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
    "APPINSIGHTS_INSTRUMENTATIONKEY": "<your-instrumentation-key>",
    "GPT_API_KEY": "<your-gpt-api-key>",
    "GPT_API_BASE_URL": "https://api.openai.com",
    "GPT_API_COMPLETION_ENDPOINT": "/v1/completions",
    "EMAIL_ACCOUNT": "<your-email-to-send-itinerary-from>",
    "EMAIL_PASSWORD": "<your-email->",
    "SMTP_HOST": "<your-smtp-host>",
    "SMTP_PORT": "<your-smtp-port>"
  }
}
```
6. Start Debugging

**NB:**
- `GPT_API_KEY` can be obtained from OpenAI by subscribing to their API service
- If perhaps the `GPT_API_BASE_URL`, `GPT_API_COMPLETION_ENDPOINT` has changed as at the time of running this code, update the values in the template as necessary.
- The `EMAIL_PASSWORD` in the template refers to [App Password](https://support.google.com/mail/answer/185833?hl=en). I had to use the App password because I could not authenticate with my google account password. It may not be the best option but it is open for improvement.

# 2. To run on Azure Cloud
This repository includes a GitAction workflow to automatically deploy the app on Azure automatically on push to master with some minor updates to the workflow and repository secrets.

### Requirements
- GitHub Account
- Azure account + subscription

### Steps
1. Create a service principal and assign a `contributor`` role to it. [See Link](https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal).
2. Create a new service principal client secret. [See Link](https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal#option-3-create-a-new-client-secret). Ensure to copy the secret immediately after creation

NB: GitAction uses this service principal to authenticate to Azure cloud to setup infrastructure and deploy the function app.

3. Fork this repository and create the following repository secrets:

- AZURE_CREDENTIALS = `{"clientSecret":  "******", "subscriptionId":  "******", "tenantId":  "******", "clientId":  "******"}`
- EMAIL_ACCOUNT = *****
- EMAIL_PASSWORD = ****
- GPT_API_KEY = ****

4. Customize the branch to trigger to workflow and `env` section of the workflow file - `.github/workflows/deploy_azure_function_app.yml` as needed.

5. push changes to the branch for which the workflow is configured.


**NB:**
- The `subscriptionId`, `tenantId` values are specific to the Azure Account. The `clientId` refers to the ApplicationId of the service principal(App registered) in step 1 and the `clientSecret` refers to the secret created in step 2.
- `GPT_API_KEY` can be obtained from OpenAI by subscribing to their API service
- If perhaps the `GPT_API_BASE_URL`, `GPT_API_COMPLETION_ENDPOINT` has changed as at the time of running this code, update the values in the env section of the workflow file.
- The `EMAIL_PASSWORD` in the template refers to [App Password](https://support.google.com/mail/answer/185833?hl=en). I had to use the App password because I could not authenticate with my google account password. It may not be the best option but it is open for improvement.
- Ensure to make the Azure names as unique as possible and stick to Azure naming conventions [see link](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-name-rules)
