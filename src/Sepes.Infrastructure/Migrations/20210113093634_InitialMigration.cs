using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    StorageAccountName = table.Column<string>(maxLength: 64, nullable: false),
                    StorageAccountId = table.Column<string>(maxLength: 256, nullable: true),
                    Location = table.Column<string>(maxLength: 64, nullable: false),
                    Classification = table.Column<string>(maxLength: 32, nullable: false),
                    LRAId = table.Column<int>(nullable: false),
                    DataId = table.Column<int>(nullable: false),
                    SourceSystem = table.Column<string>(maxLength: 256, nullable: true),
                    BADataOwner = table.Column<string>(maxLength: 256, nullable: true),
                    Asset = table.Column<string>(maxLength: 256, nullable: true),
                    CountryOfOrigin = table.Column<string>(maxLength: 256, nullable: true),
                    AreaL1 = table.Column<string>(maxLength: 256, nullable: true),
                    AreaL2 = table.Column<string>(maxLength: 256, nullable: true),
                    Tags = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 64, nullable: true),
                    StudyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiskSizes",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Size = table.Column<int>(nullable: false),
                    DisplayText = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskSizes", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: true),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 4096, nullable: true),
                    ResultsAndLearnings = table.Column<string>(maxLength: 4096, nullable: true),
                    WbsCode = table.Column<string>(maxLength: 64, nullable: true),
                    Vendor = table.Column<string>(maxLength: 128, nullable: false),
                    Restricted = table.Column<bool>(nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 512, nullable: true),
                    StudySpecificDatasetsResourceGroup = table.Column<string>(maxLength: 64, nullable: true),
                    Closed = table.Column<bool>(nullable: true),
                    ClosedAt = table.Column<DateTime>(nullable: true),
                    ClosedBy = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    FullName = table.Column<string>(maxLength: 128, nullable: false),
                    UserName = table.Column<string>(maxLength: 128, nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 256, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Variables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Int1 = table.Column<int>(nullable: true),
                    Int2 = table.Column<int>(nullable: true),
                    Int3 = table.Column<int>(nullable: true),
                    Str1 = table.Column<string>(maxLength: 256, nullable: true),
                    Str2 = table.Column<string>(maxLength: 256, nullable: true),
                    Str3 = table.Column<string>(maxLength: 256, nullable: true),
                    Bool1 = table.Column<bool>(nullable: true),
                    Bool2 = table.Column<bool>(nullable: true),
                    Bool3 = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VmSizes",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Category = table.Column<string>(maxLength: 32, nullable: true),
                    DisplayText = table.Column<string>(maxLength: 128, nullable: true),
                    NumberOfCores = table.Column<int>(nullable: false),
                    OsDiskSizeInMB = table.Column<int>(nullable: false),
                    ResourceDiskSizeInMB = table.Column<int>(nullable: false),
                    MemoryGB = table.Column<int>(nullable: false),
                    MaxDataDiskCount = table.Column<int>(nullable: false),
                    MaxNetworkInterfaces = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VmSizes", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "DatasetFirewallRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    DatasetId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(maxLength: 64, nullable: true),
                    RuleType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetFirewallRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetFirewallRules_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegionDiskSize",
                columns: table => new
                {
                    RegionKey = table.Column<string>(maxLength: 64, nullable: false),
                    VmDiskKey = table.Column<string>(maxLength: 64, nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionDiskSize", x => new { x.RegionKey, x.VmDiskKey });
                    table.ForeignKey(
                        name: "FK_RegionDiskSize_Regions_RegionKey",
                        column: x => x.RegionKey,
                        principalTable: "Regions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionDiskSize_DiskSizes_VmDiskKey",
                        column: x => x.VmDiskKey,
                        principalTable: "DiskSizes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sandboxes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    TechnicalContactName = table.Column<string>(maxLength: 64, nullable: false),
                    TechnicalContactEmail = table.Column<string>(maxLength: 128, nullable: false),
                    Region = table.Column<string>(maxLength: 64, nullable: false),
                    StudyId = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sandboxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sandboxes_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyDatasets",
                columns: table => new
                {
                    StudyId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyDatasets", x => new { x.StudyId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_StudyDatasets_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyDatasets_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyParticipants",
                columns: table => new
                {
                    StudyId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    RoleName = table.Column<string>(maxLength: 64, nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyParticipants", x => new { x.StudyId, x.UserId, x.RoleName });
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionVmSize",
                columns: table => new
                {
                    RegionKey = table.Column<string>(maxLength: 64, nullable: false),
                    VmSizeKey = table.Column<string>(maxLength: 64, nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionVmSize", x => new { x.RegionKey, x.VmSizeKey });
                    table.ForeignKey(
                        name: "FK_RegionVmSize_Regions_RegionKey",
                        column: x => x.RegionKey,
                        principalTable: "Regions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionVmSize_VmSizes_VmSizeKey",
                        column: x => x.VmSizeKey,
                        principalTable: "VmSizes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CloudResources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    SandboxId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<string>(maxLength: 256, nullable: true),
                    ResourceKey = table.Column<string>(maxLength: 256, nullable: true),
                    ResourceName = table.Column<string>(maxLength: 256, nullable: true),
                    ResourceType = table.Column<string>(maxLength: 64, nullable: true),
                    ResourceGroupName = table.Column<string>(maxLength: 64, nullable: true),
                    LastKnownProvisioningState = table.Column<string>(maxLength: 64, nullable: true),
                    Tags = table.Column<string>(maxLength: 4096, nullable: true),
                    Region = table.Column<string>(maxLength: 32, nullable: true),
                    ConfigString = table.Column<string>(maxLength: 4096, nullable: true),
                    SandboxControlled = table.Column<bool>(nullable: false),
                    Deleted = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ParentResourceId = table.Column<int>(nullable: true),
                    StudyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudResources_CloudResources_ParentResourceId",
                        column: x => x.ParentResourceId,
                        principalTable: "CloudResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CloudResources_Sandboxes_SandboxId",
                        column: x => x.SandboxId,
                        principalTable: "Sandboxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CloudResources_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SandboxDatasets",
                columns: table => new
                {
                    SandboxId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false),
                    AddedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Added = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxDatasets", x => new { x.SandboxId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_SandboxDatasets_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SandboxDatasets_Sandboxes_SandboxId",
                        column: x => x.SandboxId,
                        principalTable: "Sandboxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SandboxPhaseHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    SandboxId = table.Column<int>(nullable: false),
                    Counter = table.Column<int>(nullable: false),
                    Phase = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxPhaseHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SandboxPhaseHistory_Sandboxes_SandboxId",
                        column: x => x.SandboxId,
                        principalTable: "Sandboxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloudResourceOperations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CloudResourceId = table.Column<int>(nullable: false),
                    DependsOnOperationId = table.Column<int>(nullable: true),
                    OperationType = table.Column<string>(maxLength: 32, nullable: true),
                    Status = table.Column<string>(maxLength: 32, nullable: true),
                    TryCount = table.Column<int>(nullable: false),
                    MaxTryCount = table.Column<int>(nullable: false),
                    BatchId = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedBySessionId = table.Column<string>(maxLength: 64, nullable: true),
                    CarriedOutBySessionId = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    LatestError = table.Column<string>(maxLength: 4096, nullable: true),
                    QueueMessageId = table.Column<string>(maxLength: 64, nullable: true),
                    QueueMessagePopReceipt = table.Column<string>(maxLength: 64, nullable: true),
                    QueueMessageVisibleAgainAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudResourceOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudResourceOperations_CloudResources_CloudResourceId",
                        column: x => x.CloudResourceId,
                        principalTable: "CloudResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CloudResourceOperations_CloudResourceOperations_DependsOnOperationId",
                        column: x => x.DependsOnOperationId,
                        principalTable: "CloudResourceOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CloudResourceRoleAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CloudResourceId = table.Column<int>(nullable: false),
                    UserOjectId = table.Column<string>(maxLength: 64, nullable: true),
                    RoleId = table.Column<string>(maxLength: 512, nullable: true),
                    ForeignSystemId = table.Column<string>(maxLength: 512, nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 64, nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudResourceRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudResourceRoleAssignments_CloudResources_CloudResourceId",
                        column: x => x.CloudResourceId,
                        principalTable: "CloudResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceOperations_CloudResourceId",
                table: "CloudResourceOperations",
                column: "CloudResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceOperations_DependsOnOperationId",
                table: "CloudResourceOperations",
                column: "DependsOnOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceRoleAssignments_CloudResourceId",
                table: "CloudResourceRoleAssignments",
                column: "CloudResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_ParentResourceId",
                table: "CloudResources",
                column: "ParentResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResources",
                column: "SandboxId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_StudyId",
                table: "CloudResources",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetFirewallRules_DatasetId",
                table: "DatasetFirewallRules",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionDiskSize_VmDiskKey",
                table: "RegionDiskSize",
                column: "VmDiskKey");

            migrationBuilder.CreateIndex(
                name: "IX_RegionVmSize_VmSizeKey",
                table: "RegionVmSize",
                column: "VmSizeKey");

            migrationBuilder.CreateIndex(
                name: "IX_SandboxDatasets_DatasetId",
                table: "SandboxDatasets",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sandboxes_StudyId",
                table: "Sandboxes",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_SandboxPhaseHistory_SandboxId",
                table: "SandboxPhaseHistory",
                column: "SandboxId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyDatasets_DatasetId",
                table: "StudyDatasets",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_UserId",
                table: "StudyParticipants",
                column: "UserId");

            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'norwayeast') BEGIN INSERT INTO [dbo].[Regions] VALUES ('norwayeast', getutcdate(), 'migration', 'Norway East', 0) END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'northeurope') BEGIN INSERT INTO [dbo].[Regions] VALUES ('northeurope', getutcdate(), 'migration', 'North Europe', 1) END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'westeurope') BEGIN INSERT INTO [dbo].[Regions] VALUES ('westeurope', getutcdate(), 'migration', 'West Europe', 1) END");

            migrationBuilder.Sql($"INSERT INTO Variables (Name, Description, Int1, Int2, Created, CreatedBy, Updated, UpdatedBy) VALUES ('{VariableNames.BastionTimeoutAndRetryCount}', 'Int1 is timeout in seconds, Int2 is max retry count', 600, 2, getutcdate(), 'Migration', getutcdate(), 'Migration')");
            migrationBuilder.Sql($"INSERT INTO Variables (Name, Description, Int1, Int2, Created, CreatedBy, Updated, UpdatedBy) VALUES ('{VariableNames.VmTimeoutAndRetryCount}', 'Int1 is timeout in seconds, Int2 is max retry count', 600, 2, getutcdate(), 'Migration', getutcdate(), 'Migration')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudResourceOperations");

            migrationBuilder.DropTable(
                name: "CloudResourceRoleAssignments");

            migrationBuilder.DropTable(
                name: "DatasetFirewallRules");

            migrationBuilder.DropTable(
                name: "RegionDiskSize");

            migrationBuilder.DropTable(
                name: "RegionVmSize");

            migrationBuilder.DropTable(
                name: "SandboxDatasets");

            migrationBuilder.DropTable(
                name: "SandboxPhaseHistory");

            migrationBuilder.DropTable(
                name: "StudyDatasets");

            migrationBuilder.DropTable(
                name: "StudyParticipants");

            migrationBuilder.DropTable(
                name: "Variables");

            migrationBuilder.DropTable(
                name: "CloudResources");

            migrationBuilder.DropTable(
                name: "DiskSizes");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "VmSizes");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sandboxes");

            migrationBuilder.DropTable(
                name: "Studies");
        }
    }
}
