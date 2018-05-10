// ajax lib
"use strict";

!(function (e) {
    if ("object" == typeof exports && "undefined" != typeof module) module.exports = e(); else if ("function" == typeof define && define.amd) define([], e); else {
        var t; t = "undefined" != typeof window ? window : "undefined" != typeof global ? global : "undefined" != typeof self ? self : this, t.qwest = e();
    }
})(function () {
    var e; return (function t(e, n, r) {
        function o(s, a) {
            if (!n[s]) {
                if (!e[s]) {
                    var u = "function" == typeof require && require; if (!a && u) return u(s, !0); if (i) return i(s, !0); var p = new Error("Cannot find module '" + s + "'"); throw (p.code = "MODULE_NOT_FOUND", p);
                } var f = n[s] = { exports: {} }; e[s][0].call(f.exports, function (t) {
                    var n = e[s][1][t]; return o(n ? n : t);
                }, f, f.exports, t, e, n, r);
            } return n[s].exports;
        } for (var i = "function" == typeof require && require, s = 0; s < r.length; s++) o(r[s]); return o;
    })({
        1: [function (t, n, r) {
            !(function (t) {
                "use strict"; var r = function r(e) {
                    var t = function t(e, _t, n) {
                        n = "function" == typeof n ? n() : null === n ? "" : void 0 === n ? "" : n, e[e.length] = encodeURIComponent(_t) + "=" + encodeURIComponent(n);
                    },
                        n = function n(e, r, o) {
                            var i, s, a; if ("[object Array]" === Object.prototype.toString.call(r)) for (i = 0, s = r.length; s > i; i++) n(e + "[" + ("object" == typeof r[i] ? i : "") + "]", r[i], o); else if (r && "[object Object]" === r.toString()) for (a in r) r.hasOwnProperty(a) && (e ? n(e + "[" + a + "]", r[a], o, t) : n(a, r[a], o, t)); else if (e) t(o, e, r); else for (a in r) t(o, a, r[a]); return o;
                        }; return n("", e, []).join("&").replace(/%20/g, "+");
                }; "object" == typeof n && "object" == typeof n.exports ? n.exports = r : "function" == typeof e && e.amd ? e([], function () {
                    return r;
                }) : t.param = r;
            })(this);
        }, {}], 2: [function (e, t, n) {
            !(function (e) {
                function t(e) {
                    return "function" == typeof e;
                } function n(e) {
                    return "object" == typeof e;
                } function r(e) {
                    "undefined" != typeof setImmediate ? setImmediate(e) : "undefined" != typeof process && process.nextTick ? process.nextTick(e) : setTimeout(e, 0);
                } var o; e[0][e[1]] = function i(e) {
                    var s,
                        a = [],
                        u = [],
                        p = function p(e, t) {
                            return null == s && null != e && (s = e, a = t, u.length && r(function () {
                                for (var e = 0; e < u.length; e++) u[e]();
                            })), s;
                        }; return p.then = function (p, f) {
                            var c = i(e),
                                l = function l() {
                                    function e(r) {
                                        var i,
                                            s = 0; try {
                                                if (r && (n(r) || t(r)) && t(i = r.then)) {
                                                    if (r === c) throw new TypeError(); i.call(r, function () {
                                                        s++ || e.apply(o, arguments);
                                                    }, function (e) {
                                                        s++ || c(!1, [e]);
                                                    });
                                                } else c(!0, arguments);
                                            } catch (a) {
                                                s++ || c(!1, [a]);
                                            }
                                    } try {
                                        var r = s ? p : f; t(r) ? e(r.apply(o, a || [])) : c(s, a);
                                    } catch (i) {
                                        c(!1, [i]);
                                    }
                                }; return null != s ? r(l) : u.push(l), c;
                        }, e && (p = e(p)), p;
                };
            })("undefined" == typeof t ? [window, "pinkySwear"] : [t, "exports"]);
        }, {}], qwest: [function (e, t, n) {
            t.exports = (function () {
                var t = "undefined" != typeof window ? window : self,
                    n = e("pinkyswear"),
                    r = e("jquery-param"),
                    o = {},
                    i = "json",
                    s = "post",
                    a = null,
                    u = 0,
                    p = [],
                    f = t.XMLHttpRequest ? function () {
                        return new t.XMLHttpRequest();
                    } : function () {
                        return new ActiveXObject("Microsoft.XMLHTTP");
                    },
                    c = "" === f().responseType,
                    l = function l(e, _l, d, y, m) {
                        e = e.toUpperCase(), d = d || null, y = y || {}; for (var h in o) if (!(h in y)) if ("object" == typeof o[h] && "object" == typeof y[h]) for (var T in o[h]) y[h][T] = o[h][T]; else y[h] = o[h]; var w,
                            g,
                            v,
                            x,
                            b,
                            j = !1,
                            q = !1,
                            C = !1,
                            O = 0,
                            D = {},
                            E = { text: "*/*", xml: "text/xml", json: "application/json", post: "application/x-www-form-urlencoded", document: "text/html" },
                            M = { text: "*/*", xml: "application/xml; q=1.0, text/xml; q=0.8, */*; q=0.1", json: "application/json; q=1.0, text/*; q=0.8, */*; q=0.1" },
                            X = !1,
                            L = n(function (n) {
                                return n.abort = function () {
                                    C || (g && 4 != g.readyState && g.abort(), X && (--u, X = !1), C = !0);
                                }, n.send = function () {
                                    if (!X) {
                                        if (u == a) return void p.push(n); if (C) return void (p.length && p.shift().send()); if ((++u, X = !0, g = f(), w && ("withCredentials" in g || !t.XDomainRequest || (g = new XDomainRequest(), q = !0, "GET" != e && "POST" != e && (e = "POST"))), q ? g.open(e, _l) : (g.open(e, _l, y.async, y.user, y.password), c && y.async && (g.withCredentials = y.withCredentials)), !q)) for (var r in D) D[r] && g.setRequestHeader(r, D[r]); if (c && "auto" != y.responseType) try {
                                            g.responseType = y.responseType, j = g.responseType == y.responseType;
                                        } catch (o) { } c || q ? (g.onload = R, g.onerror = S, q && (g.onprogress = function () { })) : g.onreadystatechange = function () {
                                            4 == g.readyState && R();
                                        }, y.async ? "timeout" in g ? (g.timeout = y.timeout, g.ontimeout = k) : v = setTimeout(k, y.timeout) : q && (g.ontimeout = function () { }), "auto" != y.responseType && "overrideMimeType" in g && g.overrideMimeType(E[y.responseType]), m && m(g), q ? setTimeout(function () {
                                            g.send("GET" != e ? d : null);
                                        }, 0) : g.send("GET" != e ? d : null);
                                    }
                                }, n;
                            }),
                            R = function R() {
                                var e; if ((X = !1, clearTimeout(v), p.length && p.shift().send(), !C)) {
                                    --u; try {
                                        if (j && "response" in g && null !== g.response) b = g.response; else {
                                            if ((e = y.responseType, "auto" == e)) if (q) e = i; else {
                                                var n = g.getResponseHeader("Content-Type") || ""; e = n.indexOf(E.json) > -1 ? "json" : n.indexOf(E.xml) > -1 ? "xml" : "text";
                                            } switch (e) {
                                                case "json":
                                                    if (g.responseText.length) try {
                                                        b = "JSON" in t ? JSON.parse(g.responseText) : new Function("return (" + g.responseText + ")")();
                                                    } catch (r) {
                                                        throw "Error while parsing JSON body : " + r;
                                                    } break; case "xml":
                                                    try {
                                                        t.DOMParser ? b = new DOMParser().parseFromString(g.responseText, "text/xml") : (b = new ActiveXObject("Microsoft.XMLDOM"), b.async = "false", b.loadXML(g.responseText));
                                                    } catch (r) {
                                                        b = void 0;
                                                    } if (!b || !b.documentElement || b.getElementsByTagName("parsererror").length) throw "Invalid XML"; break; default:
                                                    b = g.responseText;
                                            }
                                        } if ("status" in g && !/^2|1223/.test(g.status)) throw g.status + " (" + g.statusText + ")"; L(!0, [g, b]);
                                    } catch (r) {
                                        L(!1, [r, g, b]);
                                    }
                                }
                            },
                            S = function S(e) {
                                C || (e = "string" == typeof e ? e : "Connection aborted", L.abort(), L(!1, [new Error(e), g, null]));
                            },
                            k = function k() {
                                C || (y.attempts && ++O == y.attempts ? S("Timeout (" + _l + ")") : (g.abort(), X = !1, L.send()));
                            }; if ((y.async = "async" in y ? !!y.async : !0, y.cache = "cache" in y ? !!y.cache : !1, y.dataType = "dataType" in y ? y.dataType.toLowerCase() : s, y.responseType = "responseType" in y ? y.responseType.toLowerCase() : "auto", y.user = y.user || "", y.password = y.password || "", y.withCredentials = !!y.withCredentials, y.timeout = "timeout" in y ? parseInt(y.timeout, 10) : 3e4, y.attempts = "attempts" in y ? parseInt(y.attempts, 10) : 1, x = _l.match(/\/\/(.+?)\//), w = x && (x[1] ? x[1] != location.host : !1), "ArrayBuffer" in t && d instanceof ArrayBuffer ? y.dataType = "arraybuffer" : "Blob" in t && d instanceof Blob ? y.dataType = "blob" : "Document" in t && d instanceof Document ? y.dataType = "document" : "FormData" in t && d instanceof FormData && (y.dataType = "formdata"), null !== d)) switch (y.dataType) {
                                case "json":
                                    d = JSON.stringify(d); break; case "post":
                                    d = r(d);
                            }if (y.headers) {
                                var P = function P(e, t, n) {
                                    return t + n.toUpperCase();
                                }; for (x in y.headers) D[x.replace(/(^|-)([^-])/g, P)] = y.headers[x];
                            } return "Content-Type" in D || "GET" == e || y.dataType in E && E[y.dataType] && (D["Content-Type"] = E[y.dataType]), D.Accept || (D.Accept = y.responseType in M ? M[y.responseType] : "*/*"), w || "X-Requested-With" in D || (D["X-Requested-With"] = "XMLHttpRequest"), y.cache || "Cache-Control" in D || (D["Cache-Control"] = "no-cache"), "GET" == e && d && "string" == typeof d && (_l += (/\?/.test(_l) ? "&" : "?") + d), y.async && L.send(), L;
                    },
                    d = function d(e) {
                        var t = [],
                            r = 0,
                            o = []; return n(function (n) {
                                var i = -1,
                                    s = function s(e) {
                                        return function (s, a, u, p) {
                                            var f = ++i; return ++r, t.push(l(e, n.base + s, a, u, p).then(function (e, t) {
                                                o[f] = arguments, --r || n(!0, 1 == o.length ? o[0] : [o]);
                                            }, function () {
                                                n(!1, arguments);
                                            })), n;
                                        };
                                    }; n.get = s("GET"), n.post = s("POST"), n.put = s("PUT"), n["delete"] = s("DELETE"), n["catch"] = function (e) {
                                        return n.then(null, e);
                                    }, n.complete = function (e) {
                                        var t = function t() {
                                            e();
                                        }; return n.then(t, t);
                                    }, n.map = function (e, t, n, r, o) {
                                        return s(e.toUpperCase()).call(this, t, n, r, o);
                                    }; for (var a in e) a in n || (n[a] = e[a]); return n.send = function () {
                                        for (var e = 0, r = t.length; r > e; ++e) t[e].send(); return n;
                                    }, n.abort = function () {
                                        for (var e = 0, r = t.length; r > e; ++e) t[e].abort(); return n;
                                    }, n;
                            });
                    },
                    y = {
                        base: "", get: function get() {
                            return d(y).get.apply(this, arguments);
                        }, post: function post() {
                            return d(y).post.apply(this, arguments);
                        }, put: function put() {
                            return d(y).put.apply(this, arguments);
                        }, "delete": function _delete() {
                            return d(y)["delete"].apply(this, arguments);
                        }, map: function map() {
                            return d(y).map.apply(this, arguments);
                        }, xhr2: c, limit: function limit(e) {
                            return a = e, y;
                        }, setDefaultOptions: function setDefaultOptions(e) {
                            return o = e, y;
                        }, setDefaultXdrResponseType: function setDefaultXdrResponseType(e) {
                            return i = e.toLowerCase(), y;
                        }, setDefaultDataType: function setDefaultDataType(e) {
                            return s = e.toLowerCase(), y;
                        }, getOpenRequests: function getOpenRequests() {
                            return u;
                        }
                    }; return y;
            })();
        }, { "jquery-param": 1, pinkyswear: 2 }]
    }, {}, [1, 2])("qwest");
});

/*! onDomReady.js 1.4.0 (c) 2013 Tubal Martin - MIT license */
; (function (a) {
    "function" == typeof define && define.amd ? define(a) : window['onDomReady'] = a();
})(function () {
    "use strict"; function s(a) {
        if (!q) {
            if (!b.body) return v(s); for (q = !0; a = r.shift();) v(a);
        }
    } function t(a) {
        (o || a.type === d || b[h] === g) && (u(), s());
    } function u() {
        o ? (b[n](l, t, e), a[n](d, t, e)) : (b[j](m, t), a[j](f, t));
    } function v(a, b) {
        setTimeout(a, +b >= 0 ? b : 1);
    } function x(a) {
        q ? v(a) : r.push(a);
    } var a = window,
        b = a.document,
        c = b.documentElement,
        d = "load",
        e = !1,
        f = "on" + d,
        g = "complete",
        h = "readyState",
        i = "attachEvent",
        j = "detachEvent",
        k = "addEventListener",
        l = "DOMContentLoaded",
        m = "onreadystatechange",
        n = "removeEventListener",
        o = (k in b),
        p = e,
        q = e,
        r = []; if (b[h] === g) v(s); else if (o) b[k](l, t, e), a[k](d, t, e); else {
            b[i](m, t), a[i](f, t); try {
                p = null == a.frameElement && c;
            } catch (w) { } p && p.doScroll && (function y() {
                if (!q) {
                    try {
                        p.doScroll("left");
                    } catch (a) {
                        return v(y, 50);
                    } u(), s();
                }
            })();
        } return x.version = "1.4.0", x.isReady = function () {
            return q;
        }, x;
});

var gigyaCms = {
    responseCodes: {
        Error: 0,
        Success: 1,
        AlreadyLoggedIn: 2,
        LogoutIfNoLoginFired: 3
    },
    loginSource: {
        Login: 0,
        GetAccountInfo: 1
    },
    loggingInClassName: 'gigya-logging-in',
    authenticated: false,
    authenticatedOnServerByCurrentPage: false,
    baseUrl: '/api/gigya/account/',
    debugMode: false,
    log: function log(message, data) {
        if (gigyaCms.debugMode && window.console) {
            if (data) {
                console.log(message, data);
            } else {
                console.log(message);
            }
        }
    },
    handleError: function handleError(msg) {
        if (msg && msg.length > 0) {
            alert(msg);
        } else {
            alert(gigyaCms.genericErrorMessage);
        }
    },
    redirectAfterLogin: function redirectAfterLogin(url) {
        if (gigyaCms.loggedInRedirectUrl != '') {
            url = gigyaCms.loggedInRedirectUrl;
        }

        if (url && url.length > 0) {
            window.location = url;
        } else {
            window.location.reload();
        }
    },
    redirectAfterEditProfile: function redirectAfterEditProfile(url) {
        // do nothing as page will be updated after next load
    },
    redirectAfterLogout: function redirectAfterLogout(url) {
        if (gigyaCms.logoutRedirectUrl != '') {
            url = gigyaCms.logoutRedirectUrl;
        }

        if (url && url.length > 0) {
            window.location = url;
        } else {
            window.location.reload();
        }
    },
    genericErrorMessage: 'Sorry an error occurred. Please try again.',
    id: '',
    loggedInRedirectUrl: '',
    logoutRedirectUrl: '',
    logoutApiAction: 'logout',
    screenSetSettings: {
        login: {
            screenSet: 'Default-RegistrationLogin'
        },
        register: {
            screenSet: 'Default-RegistrationLogin',
            startScreen: 'gigya-register-screen'
        },
        editProfile: {
            screenSet: 'Default-ProfileUpdate',
            onAfterSubmit: function onAfterSubmit(eventObj) {
                gigyaCms.onProfileUpdated(eventObj);
            }
        }
    },
    initGetAccountInfo: function initGetAccountInfo() {
        gigya.accounts.getAccountInfo({ callback: gigyaCms.onGetAccountInfo });
    },
    attachEvents: function attachEvents(elems, eventName, eventHandler) {
        for (var i = 0; i < elems.length; i++) {
            elems[i].addEventListener(eventName, eventHandler, false);
        }
    },
    applyConfig: function applyConfig() {
        gigya.accounts.addEventHandlers({
            onLogin: gigyaCms.onLogin,
            onLogout: gigyaCms.onLogout
        });

        if (window.gigyaConfig.errorMessage && window.gigyaConfig.errorMessage.length > 0) {
            gigyaCms.genericErrorMessage = window.gigyaConfig.errorMessage;
        }
        gigyaCms.id = window.gigyaConfig.id;
        gigyaCms.loggedInRedirectUrl = window.gigyaConfig.loggedInRedirectUrl;
        gigyaCms.logoutRedirectUrl = window.gigyaConfig.logoutRedirectUrl;
        gigyaCms.debugMode = window.gigyaConfig.debugMode;

        if (window.gigyaConfig.logoutApiAction) {
            gigyaCms.logoutApiAction = window.gigyaConfig.logoutApiAction;
        }

        if (window.gigyaBaseUrl) {
            gigyaCms.baseUrl = window.gigyaBaseUrl;
        }
    },
    showScreenSet: function showScreenSet(settings, event) {
        var elem = event.target;

        // update the values for the settings object. setting and unsetting allows globals settings to be used and maintained e.g. event handlers
        var currentScreenSet = settings.screenSet;
        var screenSet = elem.getAttribute('data-gigya-screen');
        if (screenSet) {
            settings.screenSet = screenSet;
        }

        var currentMobileScreenSet = settings.mobileScreenSet;
        var mobileScreenSet = elem.getAttribute('data-gigya-mobile-screen');
        if (mobileScreenSet) {
            settings.mobileScreenSet = mobileScreenSet;
        }

        var currentStartScreen = settings.startScreen;
        var startScreen = elem.getAttribute('data-gigya-start-screen');
        if (startScreen) {
            settings.startScreen = startScreen;
        }

        gigyaCms.log('screen set settings', settings);
        gigya.accounts.showScreenSet(settings);

        // reset back to globals
        settings.screenSet = currentScreenSet;
        settings.mobileScreenSet = currentMobileScreenSet;
        settings.startScreen = currentStartScreen;
    },
    initialized: false,
    init: function init() {
        if (gigyaCms.initialized) {
            gigyaCms.log('gigyaCms already initialized...ignoring');
            return;
        }

        gigyaCms.attachEvents(document.getElementsByClassName('gigya-login'), 'click', function (event) {
            gigyaCms.showScreenSet(gigyaCms.screenSetSettings.login, event);
            event.preventDefault();
            return false;
        });

        gigyaCms.attachEvents(document.getElementsByClassName('gigya-logout'), 'click', function (event) {
            gigya.accounts.logout({ callback: gigyaCms.onLogoutTriggeredByUser });
            event.preventDefault();
            return false;
        });

        gigyaCms.attachEvents(document.getElementsByClassName('gigya-register'), 'click', function (event) {
            gigyaCms.showScreenSet(gigyaCms.screenSetSettings.register, event);
            event.preventDefault();
            return false;
        });

        gigyaCms.attachEvents(document.getElementsByClassName('gigya-edit-profile'), 'click', function (event) {
            gigyaCms.showScreenSet(gigyaCms.screenSetSettings.editProfile, event);
            event.preventDefault();
            return false;
        });

        gigyaCms.initEmbeddedScreens();

        var loggedInUrlElems = document.getElementsByClassName('gigya-logged-in-url');
        for (var i = 0; i < loggedInUrlElems.length; i++) {
            if (loggedInUrlElems[i].value) {
                gigyaCms.loggedInRedirectUrl = loggedInUrlElems[i].value;
                break;
            }
        }

        var loggedOutUrlElems = document.getElementsByClassName('gigya-logged-out-url');
        for (var i = 0; i < loggedOutUrlElems.length; i++) {
            if (loggedOutUrlElems[i].value) {
                gigyaCms.logoutRedirectUrl = loggedOutUrlElems[i].value;
                break;
            }
        }

        gigyaCms.initialized = true;
    },
    initEmbeddedScreens: function initEmbeddedScreens() {
        var embeddedScreens = document.getElementsByClassName('gigya-embedded-screen');
        for (var i = 0; i < embeddedScreens.length; i++) {
            var elem = embeddedScreens[i];

            var settings = {
                containerID: elem.getAttribute('data-gigya-container-id'),
                screenSet: elem.getAttribute('data-gigya-screen'),
                mobileScreenSet: elem.getAttribute('data-gigya-mobile-screen'),
                startScreen: elem.getAttribute('data-gigya-start-screen')
            };

            if (elem.getAttribute('data-update-profile')) {
                settings.onAfterSubmit = gigyaCms.onProfileUpdated;
            }

            gigyaCms.log('applying embedded screen with settings', settings);

            gigya.accounts.showScreenSet(settings);
        }
    },
    onGetAccountInfo: function onGetAccountInfo(eventObj) {
        gigyaCms.log('onGetAccountInfo', eventObj);
        if (eventObj.errorCode == 0) {
            gigyaCms.login(eventObj, false, gigyaCms.loginSource.GetAccountInfo);
        } else {
            gigyaCms.logout(false);
        }
    },
    onProfileUpdated: function onProfileUpdated(eventObj) {
        gigyaCms.log('onProfileUpdated', eventObj);
        if (!eventObj.response || !eventObj.response.UID) {
            // invalid entry on Gigya form
            return;
        }

        var data = {
            UserId: eventObj.response.UID,
            Signature: eventObj.response.UIDSignature,
            SignatureTimestamp: eventObj.response.signatureTimestamp,
            Id: gigyaCms.id
        };

        qwest.post(gigyaCms.baseUrl + 'editprofile', data).then(function (xhr, response) {
            if (response != null && response.status == gigyaCms.responseCodes.Success) {
                gigyaCms.redirectAfterEditProfile(response.redirectUrl);
            } else {
                gigyaCms.handleError(response.errorMessage);
            }
        })["catch"](function (e, xhr, response) {
            gigyaCms.handleError();
        });
    },
    onLogin: function onLogin(eventObj) {
        gigyaCms.loginEventFired = true;
        gigyaCms.log('onLogin', eventObj);
        gigyaCms.login(eventObj, true, gigyaCms.loginSource.Login);
    },
    loginEventFired: false,
    loggingIn: false,
    login: function login(eventObj, redirectAfterLogin, loginSource) {
        gigyaCms.loggingIn = true;

        var data = {
            UserId: eventObj.UID,
            Signature: eventObj.UIDSignature,
            SignatureTimestamp: eventObj.signatureTimestamp,
            Id: gigyaCms.id,
            LoginSource: loginSource
        };

        var bodyElem = document.getElementsByTagName('body')[0];
        if (loginSource == gigyaCms.loginSource.Login && !bodyElem.classList.contains(gigyaCms.loggingInClassName)) {
            bodyElem.classList.add(gigyaCms.loggingInClassName);
        }

        qwest.post(gigyaCms.baseUrl + 'login', data).then(function (xhr, response) {
            gigyaCms.loggingIn = false;

            if (response != null) {
                switch (response.status) {
                    case gigyaCms.responseCodes.AlreadyLoggedIn:
                        gigyaCms.authenticated = true;
                        if (redirectAfterLogin === true && gigyaCms.authenticatedOnServerByCurrentPage || gigyaCms.loginEventFired) {
                            gigyaCms.redirectAfterLogin(response.redirectUrl);
                        } else {
                            bodyElem.classList.remove(gigyaCms.loggingInClassName);
                        }
                        return;
                    case gigyaCms.responseCodes.LogoutIfNoLoginFired:
                        if (!gigyaCms.loginEventFired) {
                            gigya.accounts.logout();
                            bodyElem.classList.remove(gigyaCms.loggingInClassName);
                        }
                        return;
                    case gigyaCms.responseCodes.Success:
                        gigyaCms.authenticated = true;
                        gigyaCms.authenticatedOnServerByCurrentPage = true;
                        if (redirectAfterLogin === true || gigyaCms.loginEventFired) {
                            gigyaCms.redirectAfterLogin(response.redirectUrl);
                        } else {
                            bodyElem.classList.remove(gigyaCms.loggingInClassName);
                        }
                        return;
                    case gigyaCms.responseCodes.Error:
                        gigyaCms.log('logout');
                        gigya.accounts.logout();
                        gigyaCms.handleError(response.errorMessage);
                        bodyElem.classList.remove(gigyaCms.loggingInClassName);
                        return;
                }
            } else {
                // failed to login to CMS so logout of Gigya
                gigyaCms.log('logout');
                gigya.accounts.logout();
                gigyaCms.handleError(response.errorMessage);
                bodyElem.classList.remove(gigyaCms.loggingInClassName);
            }
        })["catch"](function (e, xhr, response) {
            // Process the error
            gigyaCms.loggingIn = false;

            if (xhr.status == 0) {
                // aborting the request so no need to logout
                gigyaCms.log('login request aborted...skipping');
                bodyElem.classList.remove(gigyaCms.loggingInClassName);
                return;
            }

            var error = {
                e: e,
                xhr: xhr,
                response: response
            };

            gigyaCms.log('login error', error);
            gigya.accounts.logout();

            if (xhr.status === 500) {
                gigyaCms.handleError();
            }

            bodyElem.classList.remove(gigyaCms.loggingInClassName);
        });
    },
    onLogoutTriggeredByUser: function onLogoutTriggeredByUser(response) {
        gigyaCms.log('onLogout triggered by user');
        gigyaCms.logout(true);
    },
    onLogout: function onLogout(eventObj) {
        gigyaCms.log('onLogout', eventObj);
        gigyaCms.logout(false);
    },
    logout: function logout(redirectAfterLogout) {
        var data = { Id: gigyaCms.id, Force: redirectAfterLogout };

        qwest.post(gigyaCms.baseUrl + gigyaCms.logoutApiAction, data).then(function (xhr, response) {
            if (response != null) {
                switch (response.status) {
                    case gigyaCms.responseCodes.Success:
                        if (redirectAfterLogout) {
                            gigyaCms.redirectAfterLogout(response.redirectUrl);
                        }
                        break;
                }
            } else {
                gigyaCms.handleError(response.errorMessage);
            }
        })["catch"](function (e, xhr, response) {
            if (xhr.status === 500) {
                gigyaCms.handleError();
            }
        });
    }
};

gigyaCms.applyConfig();
if (!window.gigyaConfig || window.gigyaConfig.getInfoRequired) {
    gigyaCms.initGetAccountInfo();
}

onDomReady(function () {
    gigyaCms.init();
});

