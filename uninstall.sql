DELETE FROM tc_modules WHERE module_id LIKE 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1';
DELETE FROM tc_site_map WHERE module_id LIKE 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1';
DELETE FROM tc_panelbar_categories WHERE module_id LIKE 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1';
DELETE FROM tc_module_commands WHERE module_id LIKE 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1';
DELETE FROM ar_common_configurations WHERE moduleId LIKE 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1';
DROP TABLE tcmodule_dns_providers;