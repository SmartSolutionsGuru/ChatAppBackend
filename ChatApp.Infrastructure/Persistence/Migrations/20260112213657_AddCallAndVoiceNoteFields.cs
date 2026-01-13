using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCallAndVoiceNoteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns only if they don't already exist (to handle partial migrations)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'CallDuration')
                    ALTER TABLE [Messages] ADD [CallDuration] int NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'CallStatus')
                    ALTER TABLE [Messages] ADD [CallStatus] int NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'CallType')
                    ALTER TABLE [Messages] ADD [CallType] int NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'IsCallMessage')
                    ALTER TABLE [Messages] ADD [IsCallMessage] bit NOT NULL DEFAULT CAST(0 AS bit);
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'IsVoiceNote')
                    ALTER TABLE [Messages] ADD [IsVoiceNote] bit NOT NULL DEFAULT CAST(0 AS bit);
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'VoiceNoteDuration')
                    ALTER TABLE [Messages] ADD [VoiceNoteDuration] float NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'VoiceNoteUrl')
                    ALTER TABLE [Messages] ADD [VoiceNoteUrl] nvarchar(max) NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Messages]') AND name = 'VoiceNoteWaveform')
                    ALTER TABLE [Messages] ADD [VoiceNoteWaveform] nvarchar(max) NULL;
                    
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'LastSeenAt')
                    ALTER TABLE [AspNetUsers] ADD [LastSeenAt] datetime2 NULL;
            ");

            // Create CallRecords table only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CallRecords')
                BEGIN
                    CREATE TABLE [CallRecords] (
                        [Id] bigint NOT NULL IDENTITY,
                        [CallId] uniqueidentifier NOT NULL,
                        [ChatId] bigint NOT NULL,
                        [CallerId] nvarchar(450) NOT NULL,
                        [ReceiverId] nvarchar(450) NOT NULL,
                        [Type] int NOT NULL,
                        [Status] int NOT NULL,
                        [InitiatedAt] datetime2 NOT NULL,
                        [AnsweredAt] datetime2 NULL,
                        [EndedAt] datetime2 NOT NULL,
                        [DurationSeconds] int NOT NULL,
                        CONSTRAINT [PK_CallRecords] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_CallRecords_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE
                    );
                    
                    CREATE INDEX [IX_CallRecords_CallerId_InitiatedAt] ON [CallRecords] ([CallerId], [InitiatedAt]);
                    CREATE UNIQUE INDEX [IX_CallRecords_CallId] ON [CallRecords] ([CallId]);
                    CREATE INDEX [IX_CallRecords_ChatId] ON [CallRecords] ([ChatId]);
                    CREATE INDEX [IX_CallRecords_ReceiverId_InitiatedAt] ON [CallRecords] ([ReceiverId], [InitiatedAt]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallRecords");

            migrationBuilder.DropColumn(
                name: "CallDuration",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CallStatus",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CallType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsCallMessage",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsVoiceNote",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VoiceNoteDuration",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VoiceNoteUrl",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VoiceNoteWaveform",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "LastSeenAt",
                table: "AspNetUsers");
        }
    }
}
