var STATES = {

   adminLogin: {
      name: 'adminLogin',
      displayName: 'Admin Login',
      url: '/',
      templateUrl: 'templates/adminLogin.html',
      controller: 'adminLoginController'
   },

   adminpage: {
      name: 'adminpage',
      displayName: 'Adminpage',
      templateUrl: 'templates/adminpage.html',
      controller: 'adminpageController',
      data: { requireAdminLogin: true },

      dashboard: {
         name: 'adminpage.dashboard',
         displayName: 'Dashboard',
         url: '/dashboard',
         templateUrl: 'templates/dashboard/dashboard.html',
         controller: 'dashboardController'
      },
      managementServer: {
         name: 'adminpage.managementServer',
         displayName: 'Management Server',
         url: '/managementserver',
         templateUrl: 'templates/managementServer/managementServer.html',
         controller: 'managementServerController'
      },
      adminAccount: {
         name: 'adminpage.adminAccount',
         displayName: 'Admin Account',
         url: '/adminaccount',
         templateUrl: 'templates/account/account.html',
         controller: 'adminAccountController'
      },
      logout: {
         name: 'adminpage.logout',
         controller: 'logoutController'
      }
   }
}