# Org.OpenAPITools.Api.CommitApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetCommitByProjectAndId**](CommitApi.md#getcommitbyprojectandid) | **GET** /projects/{projectId}/commits/{commitId} | Get commit by project and ID |
| [**GetCommitsByProject**](CommitApi.md#getcommitsbyproject) | **GET** /projects/{projectId}/commits | Get commits by project |
| [**PostCommitByProject**](CommitApi.md#postcommitbyproject) | **POST** /projects/{projectId}/commits | Create commit by project |

<a id="getcommitbyprojectandid"></a>
# **GetCommitByProjectAndId**
> Commit GetCommitByProjectAndId (Guid projectId, Guid commitId)

Get commit by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **commitId** | **Guid** | ID of the commit |  |

### Return type

[**Commit**](Commit.md)

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

<a id="getcommitsbyproject"></a>
# **GetCommitsByProject**
> List&lt;Commit&gt; GetCommitsByProject (Guid projectId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get commits by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Commit&gt;**](Commit.md)

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

<a id="postcommitbyproject"></a>
# **PostCommitByProject**
> Commit PostCommitByProject (Guid projectId, Commit body, Guid branchId = null)

Create commit by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Commit**](Commit.md) |  |  |
| **branchId** | **Guid** | ID of the branch - project&#39;s default branch if unspecified | [optional]  |

### Return type

[**Commit**](Commit.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** | Created |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

