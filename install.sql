INSERT INTO tc_modules (module_id, display_name, version, enabled, config_page, component_directory,
                        security_class)
VALUES ('ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 'Subdomain', '2.0', 1, null, null, null);

# ----------------------------------------------------------------------------------------------------------------------

INSERT INTO tc_panelbar_categories (category_id, module_id, display_name, view_order, parent_category_id,
                                    parent_module_id, page_id, panelbar_icon)
VALUES (1, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 'Subdomains', 151, 6, '07405876-e8c2-4b24-a774-4ef57f596384', null,
        null);

INSERT INTO tc_site_map (page_id, module_id, parent_page_id, parent_page_module_id, category_id, url, mvc_url,
                         controller, action, display_name, page_small_icon, panelbar_icon, show_in_sidebar,
                         view_order, required_permissions, menu_required_permissions, page_manager,
                         page_search_provider, cache_name)
VALUES (1, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', null, null, null, '', '', 'Subdomain', 'Create',
        'Create Subdomain', 'MenuIcons/Base/Info24x24.png', null, 0, null, '', '', null, null, '');
INSERT INTO tc_site_map (page_id, module_id, parent_page_id, parent_page_module_id, category_id, url, mvc_url,
                         controller, action, display_name, page_small_icon, panelbar_icon, show_in_sidebar,
                         view_order, required_permissions, menu_required_permissions, page_manager,
                         page_search_provider, cache_name)
VALUES (2, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 40, '07405876-e8c2-4b24-a774-4ef57f596384', 1,
        '/Subdomain/Configure', '/Subdomain/Configure', 'Subdomain', 'Configure', 'Configure Subdomain Providers',
        'MenuIcons/Base/ServerComponents24x24.png', 'MenuIcons/Base/ServerComponents16x16.png', 1, 1071,
        '({07405876-e8c2-4b24-a774-4ef57f596384,0,8})', '({07405876-e8c2-4b24-a774-4ef57f596384,0,8})', null, null, '');

# Tables ---------------------------------------------------------------------------------------------------------------

create table tcmodule_dns_providers
(
    id                    int auto_increment
        primary key,
    typeName              text        null,
    configurationModuleId varchar(36) null,
    configurationId       int         null,
    app_data              text        null
);

INSERT INTO tcmodule_dns_providers (id, typeName, configurationModuleId, configurationId, app_data)
VALUES (1, 'TCAdminSubdomain.Dns.Providers.CloudFlareProvider, TCAdminSubdomain',
        'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 2, '');
INSERT INTO tcmodule_dns_providers (id, typeName, configurationModuleId, configurationId, app_data)
VALUES (2, 'TCAdminSubdomain.Dns.Providers.Dynv6Provider, TCAdminSubdomain', 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 3,
        '');

# Configurations -------------------------------------------------------------------------------------------------------

INSERT INTO ar_common_configurations (id, moduleId, name, typeName, contents, app_data)
VALUES (1, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 'GeneralConfiguration',
        'TCAdminSubdomain.Configurations.GeneralConfiguration, TCAdminSubdomain', '{}', '');
INSERT INTO ar_common_configurations (id, moduleId, name, typeName, contents, app_data)
VALUES (2, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 'CloudflareProvider',
        'TCAdminSubdomain.Configurations.CloudFlareConfiguration, TCAdminSubdomain', '{}', '<?xml version="1.0" encoding="utf-16" standalone="yes"?>
<values>
  <add key="AR_COMMON:ConfigurationView" value="CloudFlareConfigure" type="System.String,mscorlib" />
</values>');
INSERT INTO ar_common_configurations (id, moduleId, name, typeName, contents, app_data)
VALUES (3, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 'Dynv6Provider',
        'TCAdminSubdomain.Configurations.Dynv6Configuration, TCAdminSubdomain', '{}', '<?xml version="1.0" encoding="utf-16" standalone="yes"?>
<values>
  <add key="AR_COMMON:ConfigurationView" value="Dynv6Configure" type="System.String,mscorlib" />
</values>');

# Service Events -------------------------------------------------------------------------------------------------------

INSERT INTO tc_module_commands (command_id, module_id, is_custom, description, sender_class, command_name,
                                command_class, execute_order, enabled, can_disable, master_only)
VALUES (1, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 1, 'Subdomain - After Move',
        'TCAdmin.GameHosting.SDK.Objects.Service', 'AfterMove',
        'TCAdminSubdomain.Events.Commands.ServiceCommands, TCAdminSubdomain', 100, 1, 0, null);

INSERT INTO tc_module_commands (command_id, module_id, is_custom, description, sender_class, command_name,
                                command_class, execute_order, enabled, can_disable, master_only)
VALUES (2, 'ceb0b7e0-59f6-4290-991d-b766b30f1ff1', 1, 'Subdomain - After Delete',
        'TCAdmin.GameHosting.SDK.Objects.Service', 'AfterDelete',
        'TCAdminSubdomain.Events.Commands.ServiceCommands, TCAdminSubdomain', 100, 1, 0, null);