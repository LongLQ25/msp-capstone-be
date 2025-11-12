using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSP.Application.Models.Requests.Document;
using MSP.Application.Models.Responses.Auth;
using MSP.Application.Models.Responses.Document;
using MSP.Application.Repositories;
using MSP.Application.Services.Interfaces.Document;
using MSP.Domain.Entities;
using MSP.Shared.Common;

namespace MSP.Application.Services.Implementations.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<User> _userManager;

        public DocumentService(
            IDocumentRepository documentRepository,
            IProjectRepository projectRepository,
            UserManager<User> userManager)
        {
            _documentRepository = documentRepository;
            _projectRepository = projectRepository;
            _userManager = userManager;
        }

        public async Task<ApiResponse<GetDocumentResponse>> CreateDocumentAsync(CreateDocumentRequest request)
        {
            // Validate owner exists
            var owner = await _userManager.FindByIdAsync(request.OwnerId.ToString());
            if (owner == null)
            {
                return ApiResponse<GetDocumentResponse>.ErrorResponse(null, "Owner not found");
            }

            // Validate project exists
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            if (project == null || project.IsDeleted)
            {
                return ApiResponse<GetDocumentResponse>.ErrorResponse(null, "Project not found");
            }

            var document = new Domain.Entities.Document
            {
                Name = request.Name,
                OwnerId = request.OwnerId,
                ProjectId = request.ProjectId,
                FileUrl = request.FileUrl,
                Description = request.Description,
                Size = request.Size,
                CreatedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document);
            await _documentRepository.SaveChangesAsync();

            var response = new GetDocumentResponse
            {
                Id = document.Id,
                Name = document.Name,
                OwnerId = document.OwnerId,
                ProjectId = document.ProjectId,
                FileUrl = document.FileUrl,
                Description = document.Description,
                Size = document.Size,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                Owner = new GetUserResponse
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    FullName = owner.FullName,
                    AvatarUrl = owner.AvatarUrl,
                    Role = (await _userManager.GetRolesAsync(owner)).FirstOrDefault() ?? string.Empty
                }
            };

            return ApiResponse<GetDocumentResponse>.SuccessResponse(response, "Document created successfully");
        }

        public async Task<ApiResponse<GetDocumentResponse>> UpdateDocumentAsync(UpdateDocumentRequest request)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(request.Id);
            if (document == null || document.IsDeleted)
            {
                return ApiResponse<GetDocumentResponse>.ErrorResponse(null, "Document not found");
            }

            document.Name = request.Name;
            document.Description = request.Description;
            document.UpdatedAt = DateTime.UtcNow;

            await _documentRepository.UpdateAsync(document);
            await _documentRepository.SaveChangesAsync();

            var response = new GetDocumentResponse
            {
                Id = document.Id,
                Name = document.Name,
                OwnerId = document.OwnerId,
                ProjectId = document.ProjectId,
                FileUrl = document.FileUrl,
                Description = document.Description,
                Size = document.Size,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                Owner = document.Owner == null ? null : new GetUserResponse
                {
                    Id = document.Owner.Id,
                    Email = document.Owner.Email,
                    FullName = document.Owner.FullName,
                    AvatarUrl = document.Owner.AvatarUrl,
                    Role = (await _userManager.GetRolesAsync(document.Owner)).FirstOrDefault() ?? string.Empty
                }
            };

            return ApiResponse<GetDocumentResponse>.SuccessResponse(response, "Document updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteDocumentAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null || document.IsDeleted)
            {
                return ApiResponse<string>.ErrorResponse(null, "Document not found");
            }

            await _documentRepository.SoftDeleteAsync(document);
            await _documentRepository.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Document deleted successfully");
        }

        public async Task<ApiResponse<GetDocumentResponse>> GetDocumentByIdAsync(Guid documentId)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                return ApiResponse<GetDocumentResponse>.ErrorResponse(null, "Document not found");
            }

            var response = new GetDocumentResponse
            {
                Id = document.Id,
                Name = document.Name,
                OwnerId = document.OwnerId,
                ProjectId = document.ProjectId,
                FileUrl = document.FileUrl,
                Description = document.Description,
                Size = document.Size,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                Owner = document.Owner == null ? null : new GetUserResponse
                {
                    Id = document.Owner.Id,
                    Email = document.Owner.Email,
                    FullName = document.Owner.FullName,
                    AvatarUrl = document.Owner.AvatarUrl,
                    Role = (await _userManager.GetRolesAsync(document.Owner)).FirstOrDefault() ?? string.Empty
                }
            };

            return ApiResponse<GetDocumentResponse>.SuccessResponse(response, "Document retrieved successfully");
        }

        public async Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetAllDocumentsAsync(PagingRequest request)
        {
            var documents = await _documentRepository.FindWithIncludePagedAsync(
                predicate: d => !d.IsDeleted,
                include: query => query
                    .Include(d => d.Owner)
                    .Include(d => d.Project),
                pageNumber: request.PageIndex,
                pageSize: request.PageSize,
                asNoTracking: true
            );

            if (documents == null || !documents.Any())
            {
                return ApiResponse<PagingResponse<GetDocumentResponse>>.ErrorResponse(null, "No documents found");
            }

            var response = new List<GetDocumentResponse>();

            foreach (var document in documents)
            {
                response.Add(new GetDocumentResponse
                {
                    Id = document.Id,
                    Name = document.Name,
                    OwnerId = document.OwnerId,
                    ProjectId = document.ProjectId,
                    FileUrl = document.FileUrl,
                    Description = document.Description,
                    Size = document.Size,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt,
                    Owner = document.Owner == null ? null : new GetUserResponse
                    {
                        Id = document.Owner.Id,
                        Email = document.Owner.Email,
                        FullName = document.Owner.FullName,
                        AvatarUrl = document.Owner.AvatarUrl,
                        Role = (await _userManager.GetRolesAsync(document.Owner)).FirstOrDefault() ?? string.Empty
                    }
                });
            }

            var totalCount = await _documentRepository.CountAsync(d => !d.IsDeleted);

            var pagingResponse = new PagingResponse<GetDocumentResponse>
            {
                Items = response,
                TotalItems = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResponse<PagingResponse<GetDocumentResponse>>.SuccessResponse(pagingResponse);
        }

        public async Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetDocumentsByProjectIdAsync(PagingRequest request, Guid projectId)
        {
            // Validate project exists
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.IsDeleted)
            {
                return ApiResponse<PagingResponse<GetDocumentResponse>>.ErrorResponse(null, "Project not found");
            }

            var documents = await _documentRepository.FindWithIncludePagedAsync(
                predicate: d => d.ProjectId == projectId && !d.IsDeleted,
                include: query => query
                    .Include(d => d.Owner)
                    .Include(d => d.Project),
                pageNumber: request.PageIndex,
                pageSize: request.PageSize,
                asNoTracking: true
            );

            if (documents == null || !documents.Any())
            {
                return ApiResponse<PagingResponse<GetDocumentResponse>>.ErrorResponse(null, "No documents found for this project");
            }

            var response = new List<GetDocumentResponse>();

            foreach (var document in documents)
            {
                response.Add(new GetDocumentResponse
                {
                    Id = document.Id,
                    Name = document.Name,
                    OwnerId = document.OwnerId,
                    ProjectId = document.ProjectId,
                    FileUrl = document.FileUrl,
                    Description = document.Description,
                    Size = document.Size,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt,
                    Owner = document.Owner == null ? null : new GetUserResponse
                    {
                        Id = document.Owner.Id,
                        Email = document.Owner.Email,
                        FullName = document.Owner.FullName,
                        AvatarUrl = document.Owner.AvatarUrl,
                        Role = (await _userManager.GetRolesAsync(document.Owner)).FirstOrDefault() ?? string.Empty
                    }
                });
            }

            var totalCount = await _documentRepository.CountAsync(d => d.ProjectId == projectId && !d.IsDeleted);

            var pagingResponse = new PagingResponse<GetDocumentResponse>
            {
                Items = response,
                TotalItems = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResponse<PagingResponse<GetDocumentResponse>>.SuccessResponse(pagingResponse);
        }

        public async Task<ApiResponse<PagingResponse<GetDocumentResponse>>> GetDocumentsByOwnerIdAsync(PagingRequest request, Guid ownerId)
        {
            // Validate owner exists
            var owner = await _userManager.FindByIdAsync(ownerId.ToString());
            if (owner == null)
            {
                return ApiResponse<PagingResponse<GetDocumentResponse>>.ErrorResponse(null, "Owner not found");
            }

            var documents = await _documentRepository.FindWithIncludePagedAsync(
                predicate: d => d.OwnerId == ownerId && !d.IsDeleted,
                include: query => query
                    .Include(d => d.Owner)
                    .Include(d => d.Project),
                pageNumber: request.PageIndex,
                pageSize: request.PageSize,
                asNoTracking: true
            );

            if (documents == null || !documents.Any())
            {
                return ApiResponse<PagingResponse<GetDocumentResponse>>.ErrorResponse(null, "No documents found for this owner");
            }

            var response = new List<GetDocumentResponse>();

            foreach (var document in documents)
            {
                response.Add(new GetDocumentResponse
                {
                    Id = document.Id,
                    Name = document.Name,
                    OwnerId = document.OwnerId,
                    ProjectId = document.ProjectId,
                    FileUrl = document.FileUrl,
                    Description = document.Description,
                    Size = document.Size,
                    CreatedAt = document.CreatedAt,
                    UpdatedAt = document.UpdatedAt,
                    Owner = document.Owner == null ? null : new GetUserResponse
                    {
                        Id = document.Owner.Id,
                        Email = document.Owner.Email,
                        FullName = document.Owner.FullName,
                        AvatarUrl = document.Owner.AvatarUrl,
                        Role = (await _userManager.GetRolesAsync(document.Owner)).FirstOrDefault() ?? string.Empty
                    }
                });
            }

            var totalCount = await _documentRepository.CountAsync(d => d.OwnerId == ownerId && !d.IsDeleted);

            var pagingResponse = new PagingResponse<GetDocumentResponse>
            {
                Items = response,
                TotalItems = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return ApiResponse<PagingResponse<GetDocumentResponse>>.SuccessResponse(pagingResponse);
        }
    }
}
