# ReportGenerator.AzureBlobHistoryStorage

> Use Azure blob container as coverage history storage
> with [ReportGenerator](https://github.com/danielpalme/ReportGenerator)

## Usage

1. Reference the plugin dll: `-plugins:path/to/ReportGenerator.AzureBlobHistoryStorage.dll`.
   Note: must use absolute path.
2. Add the following parameters:

| Name                   | Required? | Description                                                              |
|------------------------|-----------|--------------------------------------------------------------------------|
| `-historycontainerurl` | Yes       | URL to the container                                                     |
| `-writesastoken`       | Yes       | SAS token with right permissions to the container                        |
| `-repositoryname`      | Yes       | Name of repository                                                       |
| `-sourcedirectory`     |           | Set the source directory if different from the current working directory |

### Full Example

```shell
reportgenerator
  "-reports:**/coverage*.xml"
  "-targetdir:coveragereport"
  "-reporttypes:Html;JsonSummary"
  "-plugins:path/to/ReportGenerator.AzureBlobHistoryStorage.dll"
  "-historycontainerurl:https://myazureaccount.blob.core.windows.net/coverage"
  "-writesastoken:sp=rw&st=2022-07-23T17:46:00Z&se=2022-07-24T01:46:00Z&spr=https&sv=2021-06-08&sr=c&sig=E%2Fbw7NG3CCRPk5vdrysaGeZx4KT9puUYtW3DSIl2NYc%3D"
  "-repositoryname:my-git-repo"
```

## Blob Naming Scheme

`{repo-name}/{commit-sha}/{file-name}`. For
example: `my-git-repo/007a8fcb3663243331747acd07317078563ef138/2021-10-07_08-02-07_CoverageHistory.xml`.
Note: file name is provided by ReportGenerator.
