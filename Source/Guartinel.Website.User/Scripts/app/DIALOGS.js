var DIALOGS = {
   base: {
      controller: 'baseDialogController',
      template: 'templates/baseDialog.html'
   },
   errorDialog: {
      headerStyle: 'md-warn',
      title: 'Error',
      controller: 'errorDialogController',
      template: 'templates/dialogs/errorDialog.html'
   },

   connectionError: {
      headerStyle: 'md-warn',
      title: 'Connection Error',
      controller: 'communicationErrorController',
      template: 'templates/dialogs/connectionErrorDialog.html'
   },

   invalidToken: {
      headerStyle: 'md-warn',
      title: 'Session Error',
      controller: 'invalidTokenDialogController',
      template: 'templates/dialogs/invalidTokenDialog.html'
   },


   packages: {
      modify: {
         title: 'Modify Packages',
         controller: 'modifyPackagesDialog',
         template: 'templates/userpage/packages/dialogs/modifyPackages.html',
         needConfirmToClose: true,
         alertDevice: {
            title: 'Modify Alert Device',
            controller: 'modifyAlertDeviceDialog',
            template: 'templates/userpage/packages/dialogs/modifyAlertDevice.html'
         },      
         emailAddress: {
            title: 'Modify Email Address',
            controller: 'modifyEmailAddressDialog',
            template: 'templates/userpage/packages/dialogs/modifyEmailAddress.html'
         }
      },
      removeAccess: {
         title: 'Remove Access',
         headerStyle: 'md-warn',
         controller: 'removeAccessDialog',
         template: 'templates/userpage/packages/dialogs/removeAccess.html'
      },
      deletePackage: {
         title: 'Delete Package',
         headerStyle: 'md-warn',
         controller: 'deletePackageDialog',
         template: 'templates/userpage/packages/dialogs/deletePackage.html'
      },
      statistics: {
         title: 'Statistics',
         controller: 'statisticsDialog',
         template: 'templates/userpage/packages/dialogs/statisticsDialog.html'
      },
      tutorialDialog: {
         title: 'Tutorial',
         controller: 'tutorialDialogController',
         template: 'templates/userpage/packages/dialogs/tutorialDialog.html'
      }
   },
   license: {
      buyLicense: {
         title: 'Buy License',
         controller: 'buyLicenseDialogController',
         template: 'templates/userpage/license/dialogs/buyLicenseDialog.html'
      },
      activateLicenseDialog: {
         title: 'Activate License',
         controller: 'activateLicenseDialogController',
         template: 'templates/userpage/license/dialogs/activateLicenseDialog.html'
      }
   },

   devices: {  
      createAndroidDevice: {
         title: 'Add Android Device',
         controller: 'createAndroidDeviceDialog',
         template: 'templates/userpage/devices/dialogs/createAndroidDevice.html',

         androidQrCode: {
            title: 'Qr Code',
            controller: 'androidQrCodeDialog',
            template: 'templates/userpage/devices/dialogs/androidQrCode.html'
         }
       },
       disconnectHardwareSensor: {
           title: 'Disconnect hardware sensor from wifi',
           controller: 'disconnectHardwareSensorDialogController',
           template: 'templates/userpage/devices/dialogs/disconnectHardwareSensor.html'

       },
      test: {
         title: 'Test Device',
         controller: 'testDeviceDialog',
         template: 'templates/userpage/devices/dialogs/testDevice.html'
      },

      deleteDevice: {
         title: 'Delete Device',
         headerStyle: 'md-warn',
         controller: 'deleteDeviceDialog',
         template: 'templates/userpage/devices/dialogs/deleteDevice.html'
      },

      editDevice: {
         title: 'Edit Device',
         controller: 'editDeviceDialogController',
         template: 'templates/userpage/devices/dialogs/editDeviceDialog.html'
      }
   },

   account: {
      created: {
         title: 'Almost Done!',
         template: 'templates/homepage/dialogs/accountCreated.html',
         controller: 'accountCreatedDialogController'
      },
      activateAccount: {
         title: 'Activate Account',
         template: 'templates/userpage/account/dialogs/activateAccountDialog.html',
         controller: 'activateAccountDialogController'
      },
      deleteAccount: {
         headerStyle: 'md-warn',
         controller: 'deleteAccountDialog',
         title: 'Are you sure to delete your account?',
         template: 'templates/userpage/account/dialogs/deleteAccountDialog.html'
      },
      freezeAccount: {
         headerStyle: 'md-warn',
         controller: 'freezeAccountDialog',
         title: 'Are you sure to freeze your account?',
         template: 'templates/userpage/account/dialogs/freezeAccountDialog.html'
      },
      update: {
         controller: 'updateAccountDialog',
         title: 'Are you sure to update your account?',
         template: 'templates/userpage/account/dialogs/updateAccount.html'
      },
      expired: {
         headerStyle: 'md-warn',
         title: 'Account\'s email is not confirmed yet!',
         template: 'templates/homepage/dialogs/accountExpired.html',
         controller: 'accountExpiredDialog'
      }
   }
}