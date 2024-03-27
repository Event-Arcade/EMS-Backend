EXEC msdb.dbo.rds_restore_database
	@restore_db_name='emsdb-v1',
	@s3_arn_to_restore_from='arn:aws:s3:::ems-db-backup-v2/dev-backups/emsdb-backup-v1.bak',
	@type='FULL';

EXEC msdb.dbo.rds_task_status @task_id=1;