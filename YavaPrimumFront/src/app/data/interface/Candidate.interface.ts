import { Tasks } from "./Tasks.interface";
import { User } from "./User.interface";

export interface Candidate 
{
  surname: string;
  firstName: string;
  patronymic: string;
  email: string;
  phone: string;
  post: string;
  country: string;

}

export interface CandidatesFullData
{
  candidate: Candidate;
  tasks: Tasks[]
}
