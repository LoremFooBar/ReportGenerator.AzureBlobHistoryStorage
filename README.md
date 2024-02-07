# ReportGenerator.AzureBlobHistoryStorage

> Use Azure blob container as coverage history storage
> with [ReportGenerator](https://github.com/danielpalme/ReportGenerator)

## Usage

1. Reference the plugin dll: `-plugins:path/to/ReportGenerator.AzureBlobHistoryStorage.dll`.
   Note: must use absolute path.
2. Configure using command line parameters to the `reportgenerator` command, or environment variables:

| Parameter Name         | Environment Variable | Required? | Description                                                                                                                |
|------------------------|----------------------|-----------|----------------------------------------------------------------------------------------------------------------------------|
| `-historycontainerurl` | HISTORYCONTAINERURL  | Yes       | URL to the container                                                                                                       |
| `-writesastoken`       | WRITESASTOKEN        | Yes       | SAS token with write permissions to the container                                                                          |
| `-repositoryname`      | REPOSITORYNAME       | Yes       | Name of the repository                                                                                                     |
| `-sourcedirectory`     | SOURCEDIRECTORY      |           | Path to source directory, if different from the working directory                                                          |
| `-commitids`           | COMMITIDS            |           | List of commit IDs separated by `,`, `\n`, or ` ` (space). Provide this if you don't want the plugin to run `git` commands |

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
  "-commitids:$(git log --format="%H" -n 50 | tr '\n' ',')"
```

## Blob Naming Scheme

`{repo-name}/{commit-sha}/{file-name}`. For
example: `my-git-repo/007a8fcb3663243331747acd07317078563ef138/2021-10-07_08-02-07_CoverageHistory.xml`.
Note: file name is provided by ReportGenerator.
