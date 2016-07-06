var gigyaSitefinity = {
    responseCodes: {
        Error: 0,
        Success: 1,
        AlreadyLoggedIn: 2
    },
    authenticated: false,
    baseUrl: '/api/gigya/account/',
    debugMode: false,
    log: function(message, data) {
        if (gigyaSitefinity.debugMode && window.console) {
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
            alert(gigyaSitefinity.genericErrorMessage);
        }
    },
    redirectAfterLogin: function (url) {
        if (gigyaSitefinity.loggedInInRedirectUrl != '') {
            url = gigyaSitefinity.loggedInInRedirectUrl;
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
        if (gigyaSitefinity.logoutRedirectUrl != '') {
            url = gigyaSitefinity.logoutRedirectUrl;
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
                gigyaSitefinity.onProfileUpdated(eventObj);
            }
        }
    },
    initGetAccountInfo: function () {
        gigya.accounts.getAccountInfo({ callback: gigyaSitefinity.onGetAccountInfo });
    },
    init: function () {
        gigyaSitefinity.genericErrorMessage = jQuery('#gigya-error-message').val();
        gigyaSitefinity.siteId = jQuery('#gigya-site-id').val();
        gigyaSitefinity.loggedInInRedirectUrl = jQuery('#gigya-redirect-url').val();
        gigyaSitefinity.logoutRedirectUrl = jQuery('#gigya-logout-url').val();
        gigyaSitefinity.debugMode = window.gigyaDebugMode;
        if (window.gigyaBaseUrl) {
            gigyaSitefinity.baseUrl = window.gigyaBaseUrl;
        }

        jQuery('#gigya-login').click(function () {
            gigya.accounts.showScreenSet(gigyaSitefinity.screenSetSettings.login);
            return false;
        });

        jQuery('#gigya-logout').click(function () {
            gigya.accounts.logout();
            return false;
        });

        jQuery('#gigya-register').click(function () {
            gigya.accounts.showScreenSet(gigyaSitefinity.screenSetSettings.register);
            return false;
        });

        jQuery('#gigya-edit-profile').click(function () {
            gigya.accounts.showScreenSet(gigyaSitefinity.screenSetSettings.editProfile);
            return false;
        });

        gigya.accounts.addEventHandlers({
            onLogin: gigyaSitefinity.onLogin,
            onLogout: gigyaSitefinity.onLogout
        });
    },
    onGetAccountInfo: function (eventObj) {
        gigyaSitefinity.log('onGetAccountInfo', eventObj);
        if (eventObj.errorCode == 0) {
            gigyaSitefinity.login(eventObj, false);
        } else {
            gigyaSitefinity.logout(false);
        }
    },
    onProfileUpdated: function (eventObj) {
        gigyaSitefinity.log('onProfileUpdated', eventObj);
        if (!eventObj.response || !eventObj.response.UID) {
            // invalid entry on Gigya form
            return;
        }

        var request = {
            UserId: eventObj.response.UID,
            Signature: eventObj.response.UIDSignature,
            SignatureTimestamp: eventObj.response.signatureTimestamp,
            SiteId: gigyaSitefinity.siteId
        };

        jQuery.ajax({
            type: 'POST',
            url: gigyaSitefinity.baseUrl + 'editprofile',
            data: request,
            success: function (data) {
                if (data != null && data.status == gigyaSitefinity.responseCodes.Success) {
                    gigyaSitefinity.redirectAfterEditProfile(data.redirectUrl);
                } else {
                    gigyaSitefinity.handleError(data.errorMessage);
                }
            },
            error: function () {
                gigyaSitefinity.handleError();
            }
        });
    },
    onLogin: function (eventObj) {
        gigyaSitefinity.log('onLogin', eventObj);
        gigyaSitefinity.login(eventObj, true);
    },
    loggingIn: false,
    login: function (eventObj, redirectAfterLogin) {
        if (gigyaSitefinity.loggingIn) {
            return;
        }

        gigyaSitefinity.loggingIn = true;

        var request = {
            UserId: eventObj.UID,
            Signature: eventObj.UIDSignature,
            SignatureTimestamp: eventObj.signatureTimestamp,
            SiteId: gigyaSitefinity.siteId
        };

        jQuery.ajax({
            type: 'POST',
            url: gigyaSitefinity.baseUrl + 'login',
            data: request,
            success: function (data) {
                gigyaSitefinity.loggingIn = false;

                if (data != null) {
                    switch (data.status) {
                        case gigyaSitefinity.responseCodes.AlreadyLoggedIn:
                            return;
                        case gigyaSitefinity.responseCodes.Success:
                            if (redirectAfterLogin) {
                                gigyaSitefinity.redirectAfterLogin(data.redirectUrl);
                            }
                            return;
                        case gigyaSitefinity.responseCodes.Error:
                            gigyaSitefinity.log('logout');
                            gigya.accounts.logout();
                            gigyaSitefinity.handleError(data.errorMessage);
                            return;
                    }
                    
                } else {
                    // failed to login to Sitefinity so logout of Gigya
                    gigyaSitefinity.log('logout');
                    gigya.accounts.logout();
                    gigyaSitefinity.handleError(data.errorMessage);
                }
            },
            error: function (jqXHR) {
                gigyaSitefinity.loggingIn = false;
                gigyaSitefinity.log('logout');
                gigya.accounts.logout();

                if (jqXHR.status === 500) {
                    gigyaSitefinity.handleError();
                }
            }
        });
    },
    onLogout: function () {
        gigyaSitefinity.log('onLogout');
        gigyaSitefinity.logout(true);
    },
    logout: function (redirectAfterLogout) {
        var request = { SiteId: gigyaSitefinity.siteId };

        jQuery.ajax({
            type: 'POST',
            url: gigyaSitefinity.baseUrl + 'logout',
            data: request,
            success: function (data) {
                if (data != null && data.status == gigyaSitefinity.responseCodes.Success) {
                    if (redirectAfterLogout) {
                        gigyaSitefinity.redirectAfterLogout(data.redirectUrl);
                    }
                } else {
                    gigyaSitefinity.handleError(data.errorMessage);
                }
            },
            error: function (jqXHR) {
                if (jqXHR.status === 500) {
                    gigyaSitefinity.handleError();
                }
            }
        });
    }
};

gigyaSitefinity.initGetAccountInfo();