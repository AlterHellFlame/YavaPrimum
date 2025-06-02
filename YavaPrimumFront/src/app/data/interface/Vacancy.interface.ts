import { User } from "./User.interface";

// Ответ с данными вакансии
export interface Vacancy {
    vacancyId: string; 
    user: User; 
    post: string;
    count: number;
    isClose: boolean;
    additionalData: string; 
}


// Запрос на создание вакансии
export interface VacancyRequest {
    post: string;
    count: number;
    additionalData: string; 
}