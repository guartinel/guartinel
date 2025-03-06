var DIALOGS = {
    base: {
        controller: 'baseDialogController',
        template: 'templates/baseDialog.html'
    },
    connectionError: {
        headerStyle: 'md-warn',
        title: 'Connection error',
        controller: 'communicationErrorController',
        template: 'templates/dialogs/connectionErrorDialog.html'
    },

    admin: {
        configure: {
            title: 'First Time Configure',
            template: 'templates/dialogs/configureAdmin.html',
            controller: 'configureAdminController',
            forced: true
        }
    },

    watcherServer: {
        add: {
            title: 'Add Watcher Server',
            controller: 'addWatcherServerDialog',
            template: 'templates/dashboard/watcherServer/dialogs/addWatcherServer.html'
        },

        edit: {
            title: 'Edit Watcher Server',
            controller: 'editWatcherServerDialog',
            template: 'templates/dashboard/watcherServer/dialogs/editWatcherServer.html'
        },

        remove: {
            title: 'Remove Watcher Server',
            headerStyle: 'md-warn',
            controller: 'removeWatcherServerDialog',
            template: 'templates/dashboard/watcherServer/dialogs/removeWatcherServer.html'
        }
    },

    userWebServer: {
        add: {
            title: 'Add User Web Server',
            controller: 'addUserWebServerDialogController',
            template: 'templates/dashboard/userWebServer/dialogs/addUserWebServerDialog.html'
        },
        edit: {
            title: 'Update User Web Server',
            controller: 'updateUserWebServerController',
            template: 'templates/dashboard/userWebServer/dialogs/updateUserWebServerDialog.html'
        },
        remove: {
            title: 'Remove User Web Server',
            controller: 'removeUserWebServerDialogController',
            template: 'templates/dashboard/userWebServer/dialogs/removeUserWebServerDialog.html'
        }
    },
    
    database: {
        update: {
            title :'Update database',
            controller : 'updateDatabaseDialogController',
            template: 'templates/dashboard/managementServer/dialogs/updateDatabaseDialog.html'
        }
    },

    managementServer: {

        add: {
            title: 'Add Management Server',
            controller: 'addManagementServerDialog',
            template: 'templates/dashboard/managementServer/dialogs/addManagementServer.html'
        },

        edit: {
            title: 'Edit Management Server',
            controller: 'editManagementServerDialog',
            template: 'templates/dashboard/managementServer/dialogs/editManagementServer.html'
        },

        remove: {
            title: 'Remove Management Server',
            headerStyle: 'md-warn',
            controller: 'removeManagementServerDialog',
            template: 'templates/dashboard/managementServer/dialogs/removeManagementServer.html'
        },

        events: {
            title: 'Management Server Events',
            controller: 'managementServerEventsController',
            template: 'templates/dashboard/managementServer/dialogs/managementServerEvents.html'
        },

        log: {
            title: 'Management Server Log',
            controller: 'managementServerLogController',
            template: 'templates/dashboard/managementserver/dialogs/managementServerLog.html'
        },

        invalidRequests: {
            title: 'Management Server Invalid Requests',
            controller: 'managementServerInvalidRequestsController',
            template: 'templates/dashboard/managementserver/dialogs/managementServerInvalidRequests.html'
        }
    }




}