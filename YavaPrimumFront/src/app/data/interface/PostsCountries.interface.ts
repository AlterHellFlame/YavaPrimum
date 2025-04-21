export interface PostsCountries {
    posts: string[];
    countries: string[];
    companies: string[];
    phoneMasks: { [country: string]: string }; // Словарь, где ключ - страна, значение - маска
}
