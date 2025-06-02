import { DateTime } from "luxon";
import { Candidate } from "./Candidate.interface";
import { User } from "./User.interface";

export interface Tasks
{
    taskId: string,
    user: User,
    candidate: Candidate,
    status: string,
    typeStatus: number,
    dateTime: DateTime,
    additionalData: string;
    isArchive: boolean
}

export interface TasksRequest
{
    candidate: Candidate,
    dateTime: string,
    post: string,
    additionalData: string;
}

