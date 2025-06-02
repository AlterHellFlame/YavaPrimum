export interface User 
{
    userId: string;
    surname: string;
    firstName: string;
    patronymic: string;
    post: string;
    company: string;
    email : string;
    phone : string;
    imgUrl : string;
}

// Пример объекта User с дефолтными значениями
export const defaultUser: User = {
    userId: '00000000-0000-0000-0000-000000000000',
    surname: '',
    firstName: '',
    patronymic: '',
    post: '',
    company: '',
    email: '',
    phone: '',
    imgUrl: 'default.png'
};