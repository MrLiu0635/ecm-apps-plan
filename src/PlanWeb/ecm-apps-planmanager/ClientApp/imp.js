(function (ua, win) {
    var imp = win.imp || {};
    imp.IAS = "com.inspur.gsp.imp.framework.plugin.GloSessionService"; //imp android service
    imp.invoke = imp.invoke || function (cls, md, pa) {
        var params = {
            className: cls,
            methodName: md,
            param: pa
        };
        var iframe = document.createElement("iframe"),
            iframeUrl = "imp://" + encodeURIComponent(JSON.stringify(params));
        iframe.setAttribute("src", iframeUrl);
        document.body.appendChild(iframe);
        iframe.parentNode.removeChild(iframe);
        iframe = null;
    };
    var os = imp.os = {};
    os.ipad = !!ua.match(/(iPad).*OS\s([\d_]+)/);
    os.iphone = !os.ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/) ? true : false;
    os.ios = os.ipad || os.iphone;
    os.isInYJ = !!ua.match(/(emmcloud)/i);
    os.android = ua.indexOf('Android') > -1;
    /**
    * 窗口相关。
    */
    imp.iWindow = {
        close: function () {
            if (!!imp.os.android) {
                imp.invokeAndReturn(imp.IAS, "getClose", '{}')
            }
            else {
                imp.invoke("WindowService", "close");
            }
        }
    };

    /**
   * GPS
   */
    imp.iGps = (function (iGps) {

        var ias = "com.inspur.imp.plugin.gps.GpsService";
        /**
         * 打开GPS
         */
        iGps.open = function () {
            if (imp.os.ios) {
                imp.invoke("GpsService", "open", null);
            } else if (imp.os.android) {
                imp.invoke(ias, "open");
            }
        };

        /**
         * 关闭GPS
         */
        iGps.close = function () {
            if (imp.os.ios) {
                imp.invoke("GpsService", "close", null);
            } else if (imp.os.android) {
                imp.invoke(ias, "close");
            }
        };

        /**
         * 获取GPS定位信息
         * 
         * @param 获得GPS定位信息回调函数，如果不传，默认是iGps.getInfoCallback
         */
        iGps.getInfo = function (callback) {
            iGps.successCallback = callback;

            // 封装参数
            var param = {
                callback: "imp.iGps.getInfoCallback"
            }

            if (imp.os.ios) {
                imp.invoke("GpsService", "getInfo", param);
            } else if (imp.os.android) {
                imp.invoke(ias,
                    "getInfo", JSON.stringify(param));
            }
        };

        /**
         * 获取GPS定位信息回调
         * 
         * @param info
         *            定位信息，json串，key分别为：latitude,longitude,satelliteNum
         */
        iGps.getInfoCallback = function (info) {
            info = eval("(" + info + ")");
            //回调
            iGps.successCallback && iGps.successCallback(info);
        };

        return iGps;
    })(imp.iGps || {});

    /**
* 二维码/条码扫描
*/
    imp.iBarCode = (function (iBarCode) {

        /**
         * 扫描二维码
         * 
         * @param callback
         *            扫描结束回调函数，如果不传，默认为iBarcode.scanCallback
         */
        iBarCode.scan = function (callback) {
            iBarCode.successCallback = callback;

            // 封装参数
            var param = {
                callback: "imp.iBarCode.scanSuccessCallback"
            };

            if (imp.os.ios) {
                imp.invoke("BarCodeService", "scan", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.barcode.scan.BarCodeService", "scan", JSON.stringify(param));
            }
        };

        /**
         * 扫描二维码回调
         * 
         * @param scanCode
         *            扫描结果
         */
        iBarCode.scanSuccessCallback = function (scanCode) {
            //回调
            iBarCode.successCallback && iBarCode.successCallback(scanCode);
        };
        window.iBarCode = iBarCode;
        return iBarCode;
    })(imp.iBarCode || {});

    /**
* 拍照
*/
    imp.iCamera = (function (iCamera) {
        var ias = "com.inspur.imp.plugin.camera.CameraService";
        var setOptions = function (op) {
            op.destinationType = 0;
            op.targetWidth = op.targetWidth || 0;
            op.targetHeight = op.targetHeight || 0;
            return op;
        };
        iCamera.DestinationType = {
            DATA_URL: 0, //将选择的图片按base64-encoded编码方式返回
            FILE_URI: 1, //将选择的图片返回其在本地的地址,即本机url
            NATIVE_URI: 2//将选择的图片返回其在项目的asset文件夹中的路径
        };

        iCamera.EncodingType = {
            JPEG: 0, //返回图片为jpeg格式
            PNG: 1//返回图片为png的格式 
        };

        /**
         * 打开相机照相
         * 
         * @param options
         *            拍照参数
         * 
         * @param successCallback   
         *            拍照成功回调函数
         * 
         * @param errorCallback
         *            拍照失败回调函数
         * 
         *
         */
        iCamera.open = function (options, successCallback, errorCallback) {
            iCamera.successCallback = successCallback;
            iCamera.errorCallback = errorCallback;
            options = setOptions(options);
            // 封装参数
            var param = {
                success: "imp.iCamera.openSuccessCallback",
                fail: "imp.iCamera.openErrorCallback",
                options: options
            };
            if (imp.os.ios) {
                imp.invoke("CameraService", "open", param);
            } else if (imp.os.android) {
                imp.invoke(ias, "open", JSON.stringify(param));
            }
        };


        /**
         * 从相册获取图片
         * 
         * @param options
         *            图片选择参数
         * @param successCallback
         *            图片选择成功回调函数 
         * 
         * @param errorCallback
         *            图片选择失败回调函数
         * 
         */
        iCamera.select = function (options, successCallback, errorCallback) {
            iCamera.chooseSuccessCallback = successCallback;
            iCamera.chooseErrorCallback = errorCallback;
            options = setOptions(options);
            // 封装参数
            var param = {
                success: "imp.iCamera.selectSuccessCallback",
                fail: "imp.iCamera.selectErrorCallback",
                options: options
            };
            if (imp.os.ios) {
                imp.invoke("CameraService", "getPicture", param);
            } else if (imp.os.android) {
                imp.invoke(ias, "getPicture", JSON.stringify(param));
            }
        };
        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iCamera.selectSuccessCallback = function (info) {

            //回调
            var data = info;
            if (typeof info != "object") {
                data = eval("(" + info + ")");
            }
            iCamera.chooseSuccessCallback && iCamera.chooseSuccessCallback(data);
        };

        iCamera.selectErrorCallback = function (info) {
            //回调
            iCamera.chooseErrorCallback && iCamera.chooseErrorCallback(info);
        };


        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iCamera.openSuccessCallback = function (info) {
            //回调
            if (typeof info != 'object') {
                info = eval("(" + info + ")");
            }

            iCamera.successCallback && iCamera.successCallback(info);
        };

        iCamera.openErrorCallback = function (info) {
            //回调
            iCamera.errorCallback && iCamera.errorCallback(info);
        };

        return iCamera;
    })(imp.iCamera || {});

    imp.iPhoto = (function (iPhoto) {

        iPhoto.DestinationType = {
            DATA_URL: 0, //将选择的图片按base64-encoded编码方式返回
            FILE_URI: 1, //将选择的图片返回其在本地的地址,即本机url
            NATIVE_URI: 2//将选择的图片返回其在项目的asset文件夹中的路径
        };

        iPhoto.EncodingType = {
            JPEG: 0, //返回图片为jpeg格式
            PNG: 1//返回图片为png的格式 
        };
        var gsPath = '/cwbase/gsp/webservice/RESTFulWebService/RestFulServiceForIMP.ashx?resource=gspiot&method=PhotoHandler&type=';
        /**
         * 打开相机照相
         * 
         * @param options
         *            拍照参数
         * 
         * @param successCallback   
         *            拍照成功回调函数
         * 
         * @param errorCallback
         *            拍照失败回调函数
         * 
         *
         */
        iPhoto.takePhotoAndUpload = function (options, successCallback, errorCallback) {
            if (!imp.os.isInYJ) {
                alert('takePhotoAndUpload is only support bu yun+');
                return;
            }
            if (!options.type) {
                alert('options must have parameters:"type" ');
                return;
            }
            if (typeof options != 'object') {
                alert('options must be a object!');
                return;
            }

            var op = options;
            op.quality = op.quality || 90;
            op.encodingType = op.encodingType || 0;

            iPhoto.successCallback = successCallback;
            iPhoto.errorCallback = errorCallback;
            var url = op.url || document.location.origin + gsPath + options.type;
            // 封装参数
            var param = {
                success: "imp.iPhoto.openSuccessCallback",
                fail: "imp.iPhoto.openErrorCallback",
                options: op,
                uploadUrl: url
            };

            if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.photo.PhotoService",
                    "takePhotoAndUpload", JSON.stringify(param));
            }
            else {
                imp.invoke("PhotoService", "takePhotoAndUpload", param);
            }
        };


        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iPhoto.openSuccessCallback = function (info) {
            //回调
            if (typeof info != 'object') {
                info = eval("(" + info + ")");
            }

            iPhoto.successCallback && iPhoto.successCallback(info);
        };

        iPhoto.openErrorCallback = function (info) {
            //回调
            iPhoto.errorCallback && iPhoto.errorCallback(info);
        };

        /**
         * 从相册获取图片
         * 
         * @param options
         *            图片选择参数
         * @param successCallback
         *            图片选择成功回调函数 
         * 
         * @param errorCallback
         *            图片选择失败回调函数
         * 
         */
        iPhoto.selectAndUpload = function (options, successCallback, errorCallback) {

            if (!imp.os.isInYJ) {
                alert('takePhotoAndUpload is only support bu yun+');
                return;
            }
            if (!options.type) {
                alert('options must have parameters:"type" ');
                return;
            }
            if (typeof options != 'object') {
                alert('options must be a object!');
                return;
            }

            var op = options;
            op.quality = op.quality || 90;
            op.encodingType = op.encodingType || 0;
            op.picTotal = op.picTotal || 6;

            iPhoto.chooseSuccessCallback = successCallback;
            iPhoto.chooseErrorCallback = errorCallback;
            var url = op.url || document.location.origin + gsPath + options.type;
            // 封装参数
            var param = {
                success: "imp.iPhoto.selectSuccessCallback",
                fail: "imp.iPhoto.selectErrorCallback",
                options: op,
                uploadUrl: url
            };

            if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.photo.PhotoService",
                    "selectAndUpload", JSON.stringify(param));
            }
            else {
                imp.invoke("PhotoService", "selectAndUpload", param);
            }

        };
        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iPhoto.selectSuccessCallback = function (info) {
            //回调
            if (typeof info != 'object') {
                info = eval("(" + info + ")");
            }
            iPhoto.chooseSuccessCallback && iPhoto.chooseSuccessCallback(info);
        };

        iPhoto.selectErrorCallback = function (info) {
            //回调
            iPhoto.chooseErrorCallback && iPhoto.chooseErrorCallback(info);
        };
        return iPhoto;
    })(imp.iPhoto || {});


    imp.iSms = (function (iSms) {
        /**
         * 打开系统发送短信的界面，根据传入参数自动填写好相关信息
         * 
         * @param telNo
         *            电话号码
         * @param msg
         *            消息内容
         */
        iSms.open = function (telNo, msg) {
            // 封装参数
            var param = {
                tel: telNo,
                msg: msg
            }

            if (imp.os.ios) {
                imp.invoke("SmsService", "open", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.sms.SmsService", "open", JSON
                    .stringify(param));
            }
        };

        /**
         * 直接发送短信
         * 
         * @param telNo
         *            电话号码
         * @param msg
         *            消息内容
         */
        iSms.send = function (telNo, msg, successCallback, errorCallback) {
            if (telNo == "") {
                window.alert("电话号码不能为空");
                return;
            }
            if (msg == "") {
                window.alert("短信内容不能为空");
                return;
            }
            iSms.successCallback = successCallback;
            iSms.errorCallback = errorCallback;
            // 封装参数
            var param = {
                tel: telNo,
                msg: msg,
                successCb: "imp.iSms.oSuccessCallback",
                errorCb: "imp.iSms.oErrorCallback"
            }

            if (imp.os.ios) {
                imp.invoke("SmsService", "send", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.sms.SmsService", "send", JSON
                    .stringify(param));
            }
        };

        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iSms.oSuccessCallback = function () {
            //回调
            iSms.successCallback && iSms.successCallback();
        };

        iSms.oErrorCallback = function () {
            //回调
            iSms.errorCallback && iSms.errorCallback();
        };




        /**
         * 群发短信
         * 
         * @param telNoArray
         *            电话号码数组
         * @param msg
         *            消息内容
         */
        iSms.batchSend = function (telNoArray, msg, successCallback, errorCallback) {
            if (telNoArray == "") {
                window.alert("电话号码不能为空");
                return;
            }
            if (msg == "") {
                window.alert("短信内容不能为空");
                return;
            }
            // 封装参数
            iSms.successCallback = successCallback;
            iSms.errorCallback = errorCallback;
            var param = {
                telArray: telNoArray,
                msg: msg,
                successCb: "imp.iSms.obSuccessCallback",
                errorCb: "imp.iSms.obErrorCallback"
            }

            if (imp.os.ios) {
                imp.invoke("SmsService", "batchSend", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.sms.SmsService", "batchSend", JSON
                    .stringify(param));
            }
        };
        /**
         * 
         * 成功回调
         * 
         * @param info
         *       输出信息
         */
        iSms.obSuccessCallback = function () {
            //回调
            iSms.successCallback && iSms.successCallback();
        };

        iSms.obErrorCallback = function () {
            //回调
            iSms.errorCallback && iSms.errorCallback();
        };
        return iSms;
    })(imp.iSms || {});

    imp.iTel = (function (iTel) {

        /**
         * 打开手机拨号界面
         * 
         * @param telNo
         *            电话号码
         */
        iTel.dial = function (telNo) {
            // 封装参数
            var param = {
                tel: telNo
            };

            if (imp.os.ios) {
                imp.invoke("TelephoneService", "dial", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.telephone.TelephoneService", "dial", JSON
                    .stringify(param));
            }
        };

        /**
         * 直接拨打电话
         * 
         * @param telNo
         * 电话号码
         */
        iTel.call = function (telNo) {
            if (telNo == "") {
                window.alert("电话号码不能为空");
                return;
            }
            // 封装参数
            var param = {
                tel: telNo
            };

            if (imp.os.ios) {
                imp.invoke("TelephoneService", "call", param);
            } else if (imp.os.android) {
                imp.invoke("com.inspur.imp.plugin.telephone.TelephoneService", "call", JSON
                    .stringify(param));
            }
        };
        return iTel;
    })(imp.iTel || {});

    imp.iFile = (function (iFile) {
        iFile.download = function (url) {
            if (/^\/cwbase/i.test(url)) {
                url = window.location.origin + url;
            }

            if (!!imp.os.android) {
                imp.invokeAndReturn(imp.IAS, "downloadFile", JSON.stringify({
                    key: url
                }));
            }
            else {
                var params_cmd = {
                    cmd: "DownloadAttachment",
                    param: url
                };
                imp.invoke(params_cmd);
            }
        };
        return iFile;
    })(imp.iFile || {});

    win.imp = imp;
})(navigator.userAgent, window);

if (typeof module === "object" && module && typeof module.exports === "object") {
    module.exports = window.imp;
} else {
    if (typeof define === "function" && define.amd) {
        define("imp", [], function () { return window.imp; });
    }
}
