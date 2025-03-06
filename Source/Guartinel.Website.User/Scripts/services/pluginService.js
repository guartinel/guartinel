app.service('pluginService', [
   '$q', 'dialogService', '$injector','pluginPackageService',
    function ($q, dialogService, $injector, pluginPackageService) {

      var availablePackages = [
        {
            type: safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.HOST_SUPERVISOR),
            name: 'Host Supervisor',
            shortName: 'Host',
            service: 'hostSupervisorPackageService',
            image: 'Content/Images/package_covers/host_supervisor.png',
            cardTemplate: 'plugins/hostSuperVisor/templates/hostSupervisorPackageCard.html',
            description: 'Monitor your host public availability with ICMP ping check or TCP connection. Great for IP cameras and servers.',
            tutorialItems: ['Content/Images/package_covers/application_supervisor.png'],
            icon: "desktop_windows"
         },
         {
            type: safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.WEBSITE_SUPERVISOR),
            name: 'Website Supervisor',
            shortName: 'Website',
            service: 'websiteSupervisorPackageService',
            image: 'Content/Images/package_covers/website_supervisor.png',
            cardTemplate: 'plugins/websiteSupervisor/templates/websiteSupervisorPackageCard.html',
            description: 'Monitor your website availability. Response time, https certificate validity and keywords on the page',
            tutorialItems: ['Content/Images/package_covers/application_supervisor.png'],
            icon: "web"
         },
         {
            type: safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR),
            name: 'Application Supervisor',
            shortName: 'Application',
            service: 'applicationSupervisorPackageService',
            image: 'Content/Images/package_covers/application_supervisor.png',
            cardTemplate: 'plugins/applicationSupervisor/templates/applicationSupervisorPackageCard.html',
            description: 'This is an application supervisor package. With this package you can monitor your application presence and get notifications about problems on the fly.',
            tutorialItems: ['Content/Images/package_covers/application_supervisor.png'],
            icon: "donut_large"
         },
         {
            type: safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.HARDWARE_SUPERVISOR),
            name: 'Hardware Supervisor',
            shortName: 'Hardware',
            service: 'hardwareSupervisorPackageService',
            image: 'Content/Images/package_covers/hardware_supervisor.png',
            cardTemplate: 'plugins/hardwareSupervisor/templates/hardwareSupervisorPackageCard.html',
            description: 'This is a hardware supervisor package. With this package you can monitor your hardware presence and get notifications about problems on the fly.',
            tutorialItems: ['Content/Images/package_covers/hardware_supervisor.png'],
            icon: "router"
         },
         {
            type: safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.EMAIL_SUPERVISOR),
            name: 'Email Supervisor',
            shortName: 'Email',
            service: 'emailSupervisorPackageService',
            image: 'Content/Images/package_covers/email_supervisor.png',
            cardTemplate: 'plugins/emailSupervisor/templates/emailSupervisorPackageCard.html',
            description: 'With this package you can monitor your email server. With this you can check your SMTP and IMAP server at once.',
            tutorialItems: ['Content/Images/package_covers/email_supervisor.png'],
            icon: "email"
         }
      ];

        this.getAvailablePackages = function() {
            return availablePackages;
        };

        this.showCreateDialog = function (availablePackage) {
            var newPack = new pluginPackageService.BasePackageTemplate({ packageType: availablePackage.type});
            //newPack.packageType = availablePackage.type;
            return $injector.get(availablePackage.service).showCreateDialog(newPack);
      };
      this.getPackageDefinition = function (packageType) {
         for (var i = 0; i < availablePackages.length; i++)
            if (availablePackages[i].type === packageType)
               return availablePackages[i];
         console.error("Couldn't find card template for package type: " + packageType);
         return null;
      }

      this.getCardTemplate = function (packageType) {
         for (var i = 0; i < availablePackages.length; i++)
            if (availablePackages[i].type === packageType)
               return availablePackages[i].cardTemplate;
         console.error("Couldn't find card template for package type: " + packageType);
         return null;
      }
   }
]);