using System;
using System.Collections.Generic;
using System.Text;

namespace NikeSonar
{
    public enum LoginResponseCode
    {
        Sucess,
        InvalidCredentials,
        DecryptFail,
        LinkedDevice,
        MaxDevices,
        JsonFail,
        UnkownFail,
        HttpError
    }
}

