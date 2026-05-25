using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FysioEnterprise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientPrefferedStaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientBirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ClientAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientLoyaltyLevel_LoyaltyLevelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientLoyaltyLevel_LoyaltyLevelDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BirthdayDiscountUsedYear = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClinicOpeningHours = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PromotionDiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotionEndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionClientID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionStaffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionRoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionInstanceTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionPromotion = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceTotal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionTypePrice = table.Column<double>(type: "float", nullable: false),
                    SessionTypeMaxAmount = table.Column<int>(type: "int", nullable: false),
                    SessionTypeTimeSpan = table.Column<TimeOnly>(type: "time", nullable: false),
                    AllowedAuthorisationNumbers = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffContactInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffAuthorisationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffAuthorisationNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Clinics_ClinicID",
                        column: x => x.ClinicID,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffClinicAssignment",
                columns: table => new
                {
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffClinicAssignment", x => new { x.StaffId, x.ClinicId });
                    table.ForeignKey(
                        name: "FK_StaffClinicAssignment_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientEmail",
                table: "Clients",
                column: "ClientEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PromotionName",
                table: "Promotions",
                column: "PromotionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PromotionStartTime_PromotionEndTime",
                table: "Promotions",
                columns: new[] { "PromotionStartTime", "PromotionEndTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_ClinicID",
                table: "Room",
                column: "ClinicID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "SessionTypes");

            migrationBuilder.DropTable(
                name: "StaffClinicAssignment");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
