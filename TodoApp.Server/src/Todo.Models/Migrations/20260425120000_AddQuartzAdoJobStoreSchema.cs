using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Todo.Models.Data;

#nullable disable

namespace Todo.Models.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260425120000_AddQuartzAdoJobStoreSchema")]
    public partial class AddQuartzAdoJobStoreSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_JOB_DETAILS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    JOB_NAME VARCHAR(200) NOT NULL,
                    JOB_GROUP VARCHAR(200) NOT NULL,
                    DESCRIPTION VARCHAR(250) NULL,
                    JOB_CLASS_NAME VARCHAR(250) NOT NULL,
                    IS_DURABLE BOOLEAN NOT NULL,
                    IS_NONCONCURRENT BOOLEAN NOT NULL,
                    IS_UPDATE_DATA BOOLEAN NOT NULL,
                    REQUESTS_RECOVERY BOOLEAN NOT NULL,
                    JOB_DATA BLOB NULL,
                    PRIMARY KEY (SCHED_NAME, JOB_NAME, JOB_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    JOB_NAME VARCHAR(200) NOT NULL,
                    JOB_GROUP VARCHAR(200) NOT NULL,
                    DESCRIPTION VARCHAR(250) NULL,
                    NEXT_FIRE_TIME BIGINT(19) NULL,
                    PREV_FIRE_TIME BIGINT(19) NULL,
                    PRIORITY INTEGER NULL,
                    TRIGGER_STATE VARCHAR(16) NOT NULL,
                    TRIGGER_TYPE VARCHAR(8) NOT NULL,
                    START_TIME BIGINT(19) NOT NULL,
                    END_TIME BIGINT(19) NULL,
                    CALENDAR_NAME VARCHAR(200) NULL,
                    MISFIRE_INSTR SMALLINT(2) NULL,
                    JOB_DATA BLOB NULL,
                    PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    FOREIGN KEY (SCHED_NAME, JOB_NAME, JOB_GROUP)
                        REFERENCES QRTZ_JOB_DETAILS(SCHED_NAME, JOB_NAME, JOB_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_SIMPLE_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    REPEAT_COUNT BIGINT(7) NOT NULL,
                    REPEAT_INTERVAL BIGINT(12) NOT NULL,
                    TIMES_TRIGGERED BIGINT(10) NOT NULL,
                    PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                        REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_CRON_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    CRON_EXPRESSION VARCHAR(120) NOT NULL,
                    TIME_ZONE_ID VARCHAR(80),
                    PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                        REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_SIMPROP_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    STR_PROP_1 VARCHAR(512) NULL,
                    STR_PROP_2 VARCHAR(512) NULL,
                    STR_PROP_3 VARCHAR(512) NULL,
                    INT_PROP_1 INT NULL,
                    INT_PROP_2 INT NULL,
                    LONG_PROP_1 BIGINT NULL,
                    LONG_PROP_2 BIGINT NULL,
                    DEC_PROP_1 NUMERIC(13, 4) NULL,
                    DEC_PROP_2 NUMERIC(13, 4) NULL,
                    BOOL_PROP_1 BOOLEAN NULL,
                    BOOL_PROP_2 BOOLEAN NULL,
                    TIME_ZONE_ID VARCHAR(80) NULL,
                    PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                        REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_BLOB_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    BLOB_DATA BLOB NULL,
                    PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    INDEX (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
                    FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                        REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_CALENDARS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    CALENDAR_NAME VARCHAR(200) NOT NULL,
                    CALENDAR BLOB NOT NULL,
                    PRIMARY KEY (SCHED_NAME, CALENDAR_NAME)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_PAUSED_TRIGGER_GRPS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    PRIMARY KEY (SCHED_NAME, TRIGGER_GROUP)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_FIRED_TRIGGERS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    ENTRY_ID VARCHAR(140) NOT NULL,
                    TRIGGER_NAME VARCHAR(200) NOT NULL,
                    TRIGGER_GROUP VARCHAR(200) NOT NULL,
                    INSTANCE_NAME VARCHAR(200) NOT NULL,
                    FIRED_TIME BIGINT(19) NOT NULL,
                    SCHED_TIME BIGINT(19) NOT NULL,
                    PRIORITY INTEGER NOT NULL,
                    STATE VARCHAR(16) NOT NULL,
                    JOB_NAME VARCHAR(200) NULL,
                    JOB_GROUP VARCHAR(200) NULL,
                    IS_NONCONCURRENT BOOLEAN NULL,
                    REQUESTS_RECOVERY BOOLEAN NULL,
                    PRIMARY KEY (SCHED_NAME, ENTRY_ID)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_SCHEDULER_STATE(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    INSTANCE_NAME VARCHAR(200) NOT NULL,
                    LAST_CHECKIN_TIME BIGINT(19) NOT NULL,
                    CHECKIN_INTERVAL BIGINT(19) NOT NULL,
                    PRIMARY KEY (SCHED_NAME, INSTANCE_NAME)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS QRTZ_LOCKS(
                    SCHED_NAME VARCHAR(120) NOT NULL,
                    LOCK_NAME VARCHAR(40) NOT NULL,
                    PRIMARY KEY (SCHED_NAME, LOCK_NAME)
                ) ENGINE=InnoDB;
                """);

            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_J_REQ_RECOVERY ON QRTZ_JOB_DETAILS(SCHED_NAME, REQUESTS_RECOVERY);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_J_GRP ON QRTZ_JOB_DETAILS(SCHED_NAME, JOB_GROUP);");

            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_J ON QRTZ_TRIGGERS(SCHED_NAME, JOB_NAME, JOB_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_JG ON QRTZ_TRIGGERS(SCHED_NAME, JOB_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_C ON QRTZ_TRIGGERS(SCHED_NAME, CALENDAR_NAME);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_G ON QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_STATE ON QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_STATE);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_N_STATE ON QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, TRIGGER_STATE);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_N_G_STATE ON QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_GROUP, TRIGGER_STATE);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_NEXT_FIRE_TIME ON QRTZ_TRIGGERS(SCHED_NAME, NEXT_FIRE_TIME);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_NFT_ST ON QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_STATE, NEXT_FIRE_TIME);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_NFT_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME, MISFIRE_INSTR, NEXT_FIRE_TIME);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME, MISFIRE_INSTR, NEXT_FIRE_TIME, TRIGGER_STATE);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE_GRP ON QRTZ_TRIGGERS(SCHED_NAME, MISFIRE_INSTR, NEXT_FIRE_TIME, TRIGGER_GROUP, TRIGGER_STATE);");

            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_TRIG_INST_NAME ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, INSTANCE_NAME);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_INST_JOB_REQ_RCVRY ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, INSTANCE_NAME, REQUESTS_RECOVERY);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_J_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, JOB_NAME, JOB_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_JG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, JOB_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_T_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP);");
            migrationBuilder.Sql("CREATE INDEX IDX_QRTZ_FT_TG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME, TRIGGER_GROUP);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_FIRED_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_PAUSED_TRIGGER_GRPS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_SCHEDULER_STATE;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_LOCKS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_SIMPLE_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_SIMPROP_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_CRON_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_BLOB_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_TRIGGERS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_JOB_DETAILS;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS QRTZ_CALENDARS;");
        }
    }
}
