var STATES = {

    homepage: {
        name: 'homepage',
        displayName: 'Homepage',
        templateUrl: 'templates/homepage/homepage.html',
        data: { requireUserLogin: false }, // children inherit data from parent
               
        getVersion: {
           name: 'homepage.getVersion',
           displayName: 'GetVersion',
           url: '/getVersion',           
           templateUrl: 'templates/homepage/getVersion.html',
           controller: 'getVersionController'
        },
        unsubscribeAllEmail: {
           name: 'homepage.unsubscribeAllEmail',
           displayName: 'UnsubscribeAllEmail',
           url: '/unsubscribeAllEmail',
           templateUrl: 'templates/homepage/unsubscribeAllEmail.html',
           controller: 'unsubscribeAllEmailController'
        },
        unsubscribeFromPackageEmail: {
           name: 'homepage.unsubscribeFromPackageEmail',
           displayName: 'UnsubscribeFromPackageEmail',
           url: '/unsubscribeFromPackageEmail',
           templateUrl: 'templates/homepage/unsubscribeFromPackageEmail.html',
           controller: 'unsubscribeFromPackageEmailController'
        },
        login: {
            name: 'homepage.login',
            displayName: 'Login',
            url: '/',
            params: {
            isInvalidToken:null
            },
            templateUrl: 'templates/homepage/login.html',
            controller: 'loginController'
        },
        register: {
            name: 'homepage.register',
            displayName: 'Register',
            url: '/register',
            templateUrl: 'templates/homepage/register.html',
            controller: 'registerController'
        },
        activate: {
            name: 'homepage.activate',
            displayName: 'Activate',
            url: '/activate/:activationCode',
            templateUrl: 'templates/homepage/activate.html',
            controller: 'accountActivationController'
        }
    },

    userpage: {
        name: 'userpage',
        displayName: 'Userpage',
        templateUrl: 'templates/userpage/userpage.html',
        controller: 'userPageController',
        data: { requireUserLogin: true }, // children inherit data from parent

        logout: {
            name: 'userpage.logout',
            controller: 'logoutController'
        },

        devices: {
            name: 'userpage.devices',
            displayName: 'DEVICES',
            url: '/devices',
            templateUrl: 'templates/userpage/devices/devices.html',
            controller: 'devicesController',
        },
        packages: {
            name: 'userpage.packages',
            displayName: 'PACKAGES',
            url: '/packages',
            templateUrl: 'templates/userpage/packages/packages.html',
            controller: 'packagesController',
            editPackage : {
                name: 'userpage.packages.editPackage',
                displayName: 'Packages',
                url: '/packages/{path:.*}',
                templateUrl: 'templates/userpage/packages/packages.html',
                controller: 'packagesController'
            }
        },
        account: {
            name: 'userpage.account',
            displayName: 'ACCOUNT',
            url: '/account',
            templateUrl: 'templates/userpage/account/account.html',
            controller: 'accountController'
        },
        license: {
            name: 'userpage.license',
            displayName: 'LICENSE',
            url: '/license',
            templateUrl: 'templates/userpage/license/license.html',
            controller: 'licenseController'
        }
    }
}