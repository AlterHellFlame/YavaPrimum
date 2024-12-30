export interface Candidate 
{
  firstName: string;
  secondName: string;
  surName: string;
  email: string;
  telephone: string;
  post: string;
  country: string;
  interviewStatus: number;

}

export function createDefaultCandidate(): Candidate {
  return {
    firstName: 'Неизвестное имя',
    secondName: 'Неизвестное отчество',
    surName: 'Неизвестная фамилия',
    email: 'email@example.com',
    telephone: '000-000-0000',
    post: 'Водитель',
    country: 'Беларусь',
    interviewStatus: 0
  };
}