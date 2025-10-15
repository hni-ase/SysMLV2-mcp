# Org.OpenAPITools.Api.BranchApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**DeleteBranchByProjectAndId**](BranchApi.md#deletebranchbyprojectandid) | **DELETE** /projects/{projectId}/branches/{branchId} | Delete branch by project and ID |
| [**GetBranchesByProject**](BranchApi.md#getbranchesbyproject) | **GET** /projects/{projectId}/branches | Get branches by project |
| [**GetBranchesByProjectAndId**](BranchApi.md#getbranchesbyprojectandid) | **GET** /projects/{projectId}/branches/{branchId} | Get branch by project and ID |
| [**PostBranchByProject**](BranchApi.md#postbranchbyproject) | **POST** /projects/{projectId}/branches | Create branch by project |

<a id="deletebranchbyprojectandid"></a>
# **DeleteBranchByProjectAndId**
> Branch DeleteBranchByProjectAndId (Guid projectId, Guid branchId)

Delete branch by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **branchId** | **Guid** | ID of the branch |  |

### Return type

[**Branch**](Branch.md)

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

<a id="getbranchesbyproject"></a>
# **GetBranchesByProject**
> List&lt;Branch&gt; GetBranchesByProject (Guid projectId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get branches by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Branch&gt;**](Branch.md)

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

<a id="getbranchesbyprojectandid"></a>
# **GetBranchesByProjectAndId**
> Branch GetBranchesByProjectAndId (Guid projectId, Guid branchId)

Get branch by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **branchId** | **Guid** | ID of the branch |  |

### Return type

[**Branch**](Branch.md)

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

<a id="postbranchbyproject"></a>
# **PostBranchByProject**
> Branch PostBranchByProject (Guid projectId, Branch body)

Create branch by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Branch**](Branch.md) |  |  |

### Return type

[**Branch**](Branch.md)

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

