export class LoginModel {
        account: string;
        password: string;
        isAdmin: boolean;
        useRefreshTokens:boolean;

        constructor(account: string, password: string) {
                this.account = account;
                this.password = password;
        }
}

export class LoginResult
{
        access_token:string;
        expires_in:number;
        nickName?:string;
        refresh_token:string;
        token_type:string;
        userName:string;
        userRoles:string;
}