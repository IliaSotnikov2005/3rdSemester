function VerifyUploadSize(UploadFieldID, MaxSize) {
    var ActualSize = 0;
    var field = document.getElementById(UploadFieldID);
    var files = field.files
    var errorMessageElement = document.getElementById("error-message");
    errorMessageElement.innerHTML = "";
    errorMessageElement.style.visibility = "hidden";

    for (let i = 0; i < files.length; ++i)
    {
        ActualSize += files[i].size;
    }

    if (ActualSize > MaxSize)
    {
        errorMessageElement.innerHTML = "Max upload size is " + parseInt(MaxSize / 1024 / 1024) + "MB," +
            " but got " + parseInt(ActualSize / 1024 / 1024) + "MB";
        errorMessageElement.style.visibility = "visible";
        return false;
    }

    return true;
}