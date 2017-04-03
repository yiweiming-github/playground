function checkUserName(inputControlId, hintControlId) {
    var userName = document.getElementById(inputControlId).value;
    var isValidFormat = checkUserNameFormat(userName);
    if (isValidFormat) {
        document.getElementById(hintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"
        checkUserNameUniqueness(inputControlId, hintControlId);
        return true;
    }
    else {
        document.getElementById(hintControlId).innerHTML = "<span style='color:#8B0000'>不正确的邮箱格式</span>"
        return false;
    }
}

function checkExistingUserName(inputControlId, hintControlId) {
    var userName = document.getElementById(inputControlId).value;
    var isValidFormat = checkUserNameFormat(userName);
    if (isValidFormat) {
        document.getElementById(hintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"        
        checkRegisteredUserName(inputControlId, hintControlId);
        return true;
    }
    else {
        document.getElementById(hintControlId).innerHTML = "<span style='color:#8B0000'>不正确的邮箱格式</span>"
        return false;
    }
}

function checkUserNameFormat(userName) {
    var regex = /^([a-zA-Z0-9_.-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/;
    if (regex.test(userName)) {
        return true;
    }
    else {
        return false;
    }
}

function checkUserNameUniqueness(inputControlId, inputHintControlId) {    
    var userName = document.getElementById(inputControlId).value;     
    $.ajax({
        url: "/services/check_user_name",
        type: "POST",
        async: false,
        data: {
            "user_name": userName,            
        },

        success: function (result) {            
            jsonObject = eval('('+ result + ')');
            var count = jsonObject["content"]["result"];
            if (count == 0) {                
                document.getElementById(inputHintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"                
            }
            else {                
                document.getElementById(inputHintControlId).innerHTML = "<span style='color:#8B0000'>该用户名已被使用</span>"                
            }

        }, error: function (jqXHR, textStatus, errorThrown) {            
        }
    });
}

function checkRegisteredUserName(inputControlId, inputHintControlId) {    
    var userName = document.getElementById(inputControlId).value;     
    $.ajax({
        url: "/services/check_user_name",
        type: "POST",
        async: false,
        data: {
            "user_name": userName,            
        },

        success: function (result) {            
            jsonObject = eval('('+ result + ')');
            var count = jsonObject["content"]["result"];            
            if (count == 1) {                
                document.getElementById(inputHintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"                
            }
            else {                
                document.getElementById(inputHintControlId).innerHTML = "<span style='color:#8B0000'>用户名不存在</span>"                
            }

        }, error: function (jqXHR, textStatus, errorThrown) {            
        }
    });
}

function checkPasswordFormat(inputControlId, inputHintControlId) {
    var password = document.getElementById(inputControlId).value
    var isValidFormat = (password != '');
    if (isValidFormat) {
        document.getElementById(inputHintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"
        return true;
    }
    else {
        document.getElementById(inputHintControlId).innerHTML = "<span style='color:#8B0000'>不正确的密码格式</span>"
        return false;
    }
}

function checkPasswordMatch(inputControlId, confirmControlId, confirmHintControlId) {
    var password = document.getElementById(inputControlId).value
    var confirmPassword = document.getElementById(confirmControlId).value
    if (password == confirmPassword) {
        document.getElementById(confirmHintControlId).innerHTML = "<span style='color:#228B22'>OK</span>"
        return true;
    }
    else {
        document.getElementById(confirmHintControlId).innerHTML = "<span style='color:#8B0000'>两次密码不一致</span>"
        return false;
    }
}

function submitRegisterInfo(inputEmailControlId, 
                            inputPasswordControlId,
                            confirmPasswordControlId,
                            inputEmailHintId,
                            inputPasswordHintId,
                            confirmPasswordHintId) {

    
    if (!checkUserName(inputEmailControlId, inputEmailHintId)) {
        return;
    }

    if (!checkPasswordFormat(inputPasswordControlId, inputPasswordHintId)) {
        return;
    }

    if (!checkPasswordMatch(
        inputPasswordControlId,
        confirmPasswordControlId,
        confirmPasswordHintId)) {
        return;
    }

    checkUserNameUniqueness(inputEmailControlId, inputEmailHintId);

    var userName = document.getElementById(inputEmailControlId).value;
    var password = document.getElementById(inputPasswordControlId).value;    
    hashValue = md5(password);    

    $.ajax({
        url: "/services/add_user",
        type: "POST",        
        data: {
            "user_name": userName,
            "password": hashValue,
            "email" : userName,
            "chinese_name" : ""
        },

        success: function (result) {            
            window.location.replace("/static/RegisterNotice.html");
        }, error: function (jqXHR, textStatus, errorThrown) {            
            window.location.replace("/static/Error.html");
        }
    })
}

function changePassword(inputEmailControlId,
                        inputPasswordControlId,
                        inputNewPasswordControlId,
                        confirmNewPasswordControlId,
                        inputEmailHintId, 
                        inputPasswordHintId, 
                        inputNewPasswordHintId,
                        confirmNewPasswordHintId) {
    if (!checkUserName(inputEmailControlId, inputEmailHintId)) {
        return;
    }

    if (!checkPasswordFormat(inputPasswordControlId, inputPasswordHintId)) {
        return;
    }

    if (!checkPasswordFormat(inputNewPasswordControlId, inputNewPasswordHintId)) {
        return;
    }

    if (!checkPasswordMatch(
        inputNewPasswordControlId,
        confirmNewPasswordControlId,
        confirmNewPasswordHintId)) {
        return;
    }

    checkExistingUserName(inputEmailControlId, inputEmailHintId);

    var userName = document.getElementById(inputEmailControlId).value;
    var password = document.getElementById(inputPasswordControlId).value;
    var newPassword = document.getElementById(inputNewPasswordControlId).value;
    hashValue = md5(password);
    newHashValue = md5(newPassword);

    $.ajax({
        url: "/services/change_password",
        type: "POST",        
        data: {
            "user_name": userName,
            "password": hashValue,
            "new_password": newHashValue
        },

        success: function (result) {
            jsonObject = eval('('+ result + ')');
            if ('error' in jsonObject['content']) {                
                document.getElementById('serverErrorMessage').innerText = jsonObject['content']['error'];
            } else {
                document.getElementById('serverErrorMessage').innerText = '';
                window.location.replace("/static/ChangePasswordNotice.html");             
            }
        }, error: function (jqXHR, textStatus, errorThrown) {            
            window.location.replace("/static/Error.html");
        }
    })
}