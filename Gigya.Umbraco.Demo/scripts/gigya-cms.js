var gigyaCms = {
    responseCodes: {
        Error: 0,
        Success: 1,
        AlreadyLoggedIn: 2
    },
    authenticated: false,
    baseUrl: '/api/gigya/account/',
    debugMode: false,
    log: function(message, data) {
        if (gigyaCms.debugMode && window.console) {
            if (data) {
                console.log(message, data);
            } else {
                console.log(message);
            }            
        }
    },
    handleError: function (msg) {
        if (msg) {
            alert(msg);
        } else {
            alert(gigyaCms.genericErrorMessage);
        }
    },
    redirectAfterLogin: function (url) {
        if (gigyaCms.loggedInInRedirectUrl != '') {
            url = gigyaCms.loggedInInRedirectUrl;
        }

        if (url && url.length > 0) {
            window.location = url;
        } else {
            window.location.reload();
        }
    },
    redirectAfterEditProfile: function (url) {
        // do nothing as page will be updated after next load
    },
    redirectAfterLogout: function (url) {
        if (gigyaCms.logoutRedirectUrl != '') {
            url = gigyaCms.logoutRedirectUrl;
        }

        if (url && url.length > 0) {
            window.location = url;
        } else {
            window.location.reload();
        }
    },
    genericErrorMessage: '',
    siteId: '',
    loggedInInRedirectUrl: '',
    logoutRedirectUrl: '',
    screenSetSettings: {
        login: {
            screenSet: 'Default-RegistrationLogin'
        },
        register: {
            screenSet: 'Default-RegistrationLogin',
            mobileScreenSet: 'DefaultMobile-RegistrationLogin',
            startScreen: 'gigya-register-screen'
        },
        editProfile: {
            screenSet: 'Default-ProfileUpdate',
            mobileScreenSet: 'DefaultMobile-ProfileUpdate',
            onAfterSubmit: function (eventObj) {
                gigyaCms.onProfileUpdated(eventObj);
            }
        }
    },
    initGetAccountInfo: function () {
        gigya.accounts.getAccountInfo({ callback: gigyaCms.onGetAccountInfo });
    },
    init: function () {
        gigyaCms.genericErrorMessage = jQuery('#gigya-error-message').val();
        gigyaCms.siteId = jQuery('#gigya-site-id').val();
        gigyaCms.loggedInInRedirectUrl = jQuery('#gigya-redirect-url').val();
        gigyaCms.logoutRedirectUrl = jQuery('#gigya-logout-url').val();
        gigyaCms.debugMode = window.gigyaDebugMode;
        if (window.gigyaBaseUrl) {
            gigyaCms.baseUrl = window.gigyaBaseUrl;
        }

        jQuery('#gigya-login').click(function () {
            gigya.accounts.showScreenSet(gigyaCms.screenSetSettings.login);
            return false;
        });

        jQuery('#gigya-logout').click(function () {
            gigya.accounts.logout();
            return false;
        });

        jQuery('#gigya-register').click(function () {
            gigya.accounts.showScreenSet(gigyaCms.screenSetSettings.register);
            return false;
        });

        jQuery('#gigya-edit-profile').click(function () {
            gigya.accounts.showScreenSet(gigyaCms.screenSetSettings.editProfile);
            return false;
        });

        gigya.accounts.addEventHandlers({
            onLogin: gigyaCms.onLogin,
            onLogout: gigyaCms.onLogout
        });
    },
    onGetAccountInfo: function (eventObj) {
        gigyaCms.log('onGetAccountInfo', eventObj);
        if (eventObj.errorCode == 0) {
            gigyaCms.login(eventObj, false);
        } else {
            gigyaCms.logout(false);
        }
    },
    onProfileUpdated: function (eventObj) {
        gigyaCms.log('onProfileUpdated', eventObj);
        if (!eventObj.response || !eventObj.response.UID) {
            // invalid entry on Gigya form
            return;
        }

        var request = {
            UserId: eventObj.response.UID,
            Signature: eventObj.response.UIDSignature,
            SignatureTimestamp: eventObj.response.signatureTimestamp,
            SiteId: gigyaCms.siteId
        };

        jQuery.ajax({
            type: 'POST',
            url: gigyaCms.baseUrl + 'editprofile',
            data: request,
            success: function (data) {
                if (data != null && data.status == gigyaCms.responseCodes.Success) {
                    gigyaCms.redirectAfterEditProfile(data.redirectUrl);
                } else {
                    gigyaCms.handleError(data.errorMessage);
                }
            },
            error: function () {
                gigyaCms.handleError();
            }
        });
    },
    onLogin: function (eventObj) {
        gigyaCms.log('onLogin', eventObj);
        gigyaCms.login(eventObj, true);
    },
    loggingIn: false,
    login: function (eventObj, redirectAfterLogin) {
        if (gigyaCms.loggingIn) {
            return;
        }

        gigyaCms.loggingIn = true;

        var request = {
            UserId: eventObj.UID,
            Signature: eventObj.UIDSignature,
            SignatureTimestamp: eventObj.signatureTimestamp,
            SiteId: gigyaCms.siteId
        };

        jQuery.ajax({
            type: 'POST',
            url: gigyaCms.baseUrl + 'login',
            data: request,
            success: function (data) {
                gigyaCms.loggingIn = false;

                if (data != null) {
                    switch (data.status) {
                        case gigyaCms.responseCodes.AlreadyLoggedIn:
                            return;
                        case gigyaCms.responseCodes.Success:
                            if (redirectAfterLogin) {
                                gigyaCms.redirectAfterLogin(data.redirectUrl);
                            }
                            return;
                        case gigyaCms.responseCodes.Error:
                            gigyaCms.log('logout');
                            gigya.accounts.logout();
                            gigyaCms.handleError(data.errorMessage);
                            return;
                    }
                    
                } else {
                    // failed to login to Sitefinity so logout of Gigya
                    gigyaCms.log('logout');
                    gigya.accounts.logout();
                    gigyaCms.handleError(data.errorMessage);
                }
            },
            error: function (jqXHR) {
                gigyaCms.loggingIn = false;
                gigyaCms.log('logout');
                gigya.accounts.logout();

                if (jqXHR.status === 500) {
                    gigyaCms.handleError();
                }
            }
        });
    },
    onLogout: function () {
        gigyaCms.log('onLogout');
        gigyaCms.logout(true);
    },
    logout: function (redirectAfterLogout) {
        var request = { SiteId: gigyaCms.siteId };

        jQuery.ajax({
            type: 'POST',
            url: gigyaCms.baseUrl + 'logout',
            data: request,
            success: function (data) {
                if (data != null && data.status == gigyaCms.responseCodes.Success) {
                    if (redirectAfterLogout) {
                        gigyaCms.redirectAfterLogout(data.redirectUrl);
                    }
                } else {
                    gigyaCms.handleError(data.errorMessage);
                }
            },
            error: function (jqXHR) {
                if (jqXHR.status === 500) {
                    gigyaCms.handleError();
                }
            }
        });
    }
};

gigyaCms.initGetAccountInfo();