import { DateTime } from "luxon";
import { Candidate } from "./Candidate.interface";

export interface Tasks
{
    /*candidate: Candidate,*/
    secondName : string,
    status: string,
    dateTime: DateTime
}