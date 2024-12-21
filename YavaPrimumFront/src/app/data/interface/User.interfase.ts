export interface User 
{
    id: number;
    firstName: string;
    secondName: string;
    surName: string;
    post: Post;
    userRegisterInfo: UserRegisterInfo;
}
  
export interface Post 
{
    id: number;
    post: string;
}
  
export interface UserRegisterInfo 
{
    id: number;
    email: string;
    password: string;
}

export interface Country
{
    id: number;
    country: string;
}
  