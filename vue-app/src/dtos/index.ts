export interface Project {
    active: boolean,
    name: string,
    connections: Connection[],
    basePath: string | undefined | null,
    templates: Template[],
    ignoreFields: string[],
    tablePrefix: string,
    variables: Variable
}

export interface Connection {
    active: boolean,
    type: string,
    host: string,
    port: string,
    user: string,
    password: string,
    databaseName: string
}

export interface Template {
    projectName: string,
    name: string,
    nameRule: string,
    sourcePath: string,
    targetPath: string,
    finalFileName: string,
    exists: boolean,
    tableName: string,
    variables: Variable
}

export interface Table {
    tableName: string,
    tableComment: string
}

export interface Variable {
    tableName?: string,
    businessName?: string,
    author?: string,
}