# Org.OpenAPITools.Api.ElementApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetElementByProjectCommitId**](ElementApi.md#getelementbyprojectcommitid) | **GET** /projects/{projectId}/commits/{commitId}/elements/{elementId} | Get element by project, commit and ID |
| [**GetElementsByProjectCommit**](ElementApi.md#getelementsbyprojectcommit) | **GET** /projects/{projectId}/commits/{commitId}/elements | Get elements by project and commit |
| [**GetRootsByProjectCommit**](ElementApi.md#getrootsbyprojectcommit) | **GET** /projects/{projectId}/commits/{commitId}/roots | Get root elements by project and commit |

<a id="getelementbyprojectcommitid"></a>
# **GetElementByProjectCommitId**
> Element GetElementByProjectCommitId (Guid projectId, Guid commitId, Guid elementId)

Get element by project, commit and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **commitId** | **Guid** | ID of the commit |  |
| **elementId** | **Guid** | ID of the element |  |

### Return type

[**Element**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getelementsbyprojectcommit"></a>
# **GetElementsByProjectCommit**
> List&lt;Element&gt; GetElementsByProjectCommit (Guid projectId, Guid commitId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get elements by project and commit


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **commitId** | **Guid** | ID of the commit |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Element&gt;**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getrootsbyprojectcommit"></a>
# **GetRootsByProjectCommit**
> List&lt;Element&gt; GetRootsByProjectCommit (Guid projectId, Guid commitId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get root elements by project and commit


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **commitId** | **Guid** | ID of the commit |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Element&gt;**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

