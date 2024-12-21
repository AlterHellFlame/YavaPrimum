import { DateTime } from "luxon";
import { Candidate } from "./Candidate.interface";
import { User } from "./User.interfase";

export interface Tasks
{
    user: User,
    candidate: Candidate,
    status: boolean
    dateTime: DateTime,
}