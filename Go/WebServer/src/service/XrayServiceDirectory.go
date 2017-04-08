package xray/service

service_directory = {
    "app_root" : "",
    "apis" : {
        // user services
        "services/test_service" : {
            "module" : "service.xray_user_services",
            "service" : "TestService",
        },
        "services/check_user_name" : {
            "module" : "service.xray_user_services",
            "service" : "CheckUserName",
        },
        "services/add_user" : {
            "module" : "service.xray_user_services",
            "service" : "AddUser",
        },
        "services/verify_registration" : {
            "module" : "service.xray_user_services",
            "service" : "VerifyRegistration",
        },
        "services/change_password" : {
            "module" : "service.xray_user_services",
            "service" : "ChangePassword",
        },

        // product services
        "services/check_latest_version" : {
            "module" : "service.xray_product_services",
            "service" : "CheckLatestVersion"
        }
    }
}