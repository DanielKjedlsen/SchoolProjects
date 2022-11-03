########################################################################################
########################################################################################
#
#   This is all the helper functions that we 
#   are using for our project
#
#
#
########################################################################################
########################################################################################

from pathlib import Path
import cleantext
from tqdm.notebook import trange, tqdm
import psycopg2
import pandas as pd
import re
import sys  



########################################################################################
########################################################################################
#Method for writing a dataframe to a csv file
def write_to_csv(df, file):
    filename = file + ".csv"
    filepath = Path(filename)
    df.to_csv(filepath, index=False)

########################################################################################
########################################################################################

def standard_clean(column):
    column = column.str.lower()

    space = r"\s\s+"
    column = column.str.replace(space, ' ')

    dunno = r"[^\-\w\s]"
    column = column.str.replace(dunno, ' ')

    column = column.str.replace('-', ' ')
    column = column.str.replace('_', ' ')
    column = column.str.replace('^', '')
    column = column.str.replace('"', '')    
    column = column.str.replace('{', '')
    column = column.str.replace('}', '')
    column = column.str.replace('."', '')
    column = column.str.replace(',,', ',')
    column = column.str.replace(',, ', ',')
    column = column.str.replace("'",'')
    column = column.str.replace("“",'')
    column = column.str.lstrip()
    return column

########################################################################################
########################################################################################

def hard_clean(column):
    column = standard_clean(column)

    # Clean date
    date = r"(((19[7-9]\d|20\d{2})|(?:jan(?:uary)?|feb(?:ruary)?|mar(?:ch)?|apr(?:il)?|may|jun(?:e)?|jul(?:y)?|aug(?:ust)?|sep(?:tember)?|oct(?:ober)?|(nov|dec)(?:ember)?)|(([12][0-9])|(3[01])|(0?[1-9])))[\/. \-,\n]){2,3}"
    column = column.str.replace(date, "<DATE>")

    # Clean email
    email = r'([\w0-9._-]+@[\w0-9._-]+\.[\w0-9_-]+)'
    column = column.str.replace(email, "<EMAIL>")

    # Clean URLs
    url = r'http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+'
    column = column.str.replace(url, "<URL>")

    # Clean numbers
    num = r'[0-9]+'
    column = column.str.replace(num, "<NUM>")

    return column

########################################################################################
########################################################################################

def time_clean(column):
    column = column.str.split(" ", 1, expand=True)[0]
    return column

########################################################################################
########################################################################################

def psyCop(Command): 
    # Connect to an existing database
    conn = None
    try:
        conn = psycopg2.connect(#database="public",
                            port = "5432",
                            host="localhost", 
                            user="postgres", 
                            password="1234")
    except psycopg2.DatabaseError as e: 
            print (f'Error {e}')
            sys.exit(1)
            
    # Open a cursor to perform database operations
    cur = conn.cursor()

    # Execute a command: this creates a new table

    cur.execute(Command)

    # Make the changes to the database persistent
    conn.commit()

    # Close communication with the database
    cur.close()
    conn.close()

########################################################################################
########################################################################################

def clean_text(content):
    # Regex er inspiret af https://medium.com/@yashj302/text-cleaning-using-regex-python-f1dded1ac5bd
    # Set all words to be lowercased
    if not isinstance(content, str):
        return content
    clean_text = content.lower()

    # Clean dates
    date = r"(((19[7-9]\d|20\d{2})|(?:jan(?:uary)?|feb(?:ruary)?|mar(?:ch)?|apr(?:il)?|may|jun(?:e)?|jul(?:y)?|aug(?:ust)?|sep(?:tember)?|oct(?:ober)?|(nov|dec)(?:ember)?)|(([12][0-9])|(3[01])|(0?[1-9])))[\/. \-,\n]){2,3}"
    clean_text = re.sub(date, ' <DATE> ', clean_text)

    # Clean email
    email1 = r'([\w0-9._-]+@[\w0-9._-]+\.[\w0-9_-]+)'
    clean_text = re.sub(email1, ' <EMAIL> ', clean_text)

    # Clean URLs
    url1 = r'http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+'
    clean_text = re.sub(url1, ' <URL> ', clean_text)

    # Clean numbers
    num1 = r'[0-9]+'
    clean_text = re.sub(num1, ' <NUM> ', clean_text)

    # Clean multiple white spaces, tabs, and newlines
    space1 = r"\s\s+"
    clean_text = re.sub(space1, ' ', clean_text)
    clean_text = clean_text.replace('\n', '')
    

    # Remove {}^"" and . since we use them as delimiter
    clean_text = clean_text.replace('^', '')
    clean_text = clean_text.replace('"', '')    
    clean_text = clean_text.replace('{', '')
    clean_text = clean_text.replace('}', '')
    clean_text = clean_text.replace('."', '')
    # Dunno
    Dunno1 = r"[^\-\w\s]"
    clean_text = re.sub(Dunno1, ' ', clean_text)
    
    return clean_text

########################################################################################
########################################################################################

#Method for creating dataframes for relationships
def create_relationship(df, column, id, name):
    # Exploding listed attributes e.i. tags, authors, keywords
    df_entity = df[["Id", column]] 
    df_entity[column] = df_entity[column].str.replace(", ,",",,")
    df_entity[column] = df_entity[column].str.split(',')
    df_entity = df_entity.explode(column)

    # Assigning id to entity 
    df_entity.rename(columns={"Id": "article_id"}, inplace=True)
    df_entity = df_entity.assign(id=(df_entity[column]).astype('category').cat.codes)
    df_article_entity = df_entity[["article_id", "id"]]
    df_article_entity = df_article_entity[df_article_entity['id'] > 0]
    df_article_entity.rename(columns={'id': id}, inplace=True)
    df_article_entity.drop_duplicates(keep='first', inplace=True)

    df_entity = df_entity[["id", column]]
    df_entity.rename(columns={column: name}, inplace=True)
    df_entity.drop_duplicates(keep='first', inplace=True)
    df_entity[name] = standard_clean(df_entity[name])
    return df_entity, df_article_entity
########################################################################################
########################################################################################

def clean_textLigt(content):
    # Regex er inspiret af https://medium.com/@yashj302/text-cleaning-using-regex-python-f1dded1ac5bd
    # Set all words to be lowercased
    if not isinstance(content, str):
        return content
    clean_text = content.lower()

    # Clean multiple white spaces, tabs, and newlines
    space1 = r"\s\s+"
    clean_text = re.sub(space1, ' ', clean_text)
    clean_text = clean_text.replace('\n', '')
    

    # Remove {}^"" and . since we use them as delimiter
    clean_text = clean_text.replace('^', '')
    clean_text = clean_text.replace('"', '')    
    clean_text = clean_text.replace('{', '')
    clean_text = clean_text.replace('}', '')
    clean_text = clean_text.replace('."', '')
    clean_text = clean_text.replace(',,', ',')
    clean_text = clean_text.replace(',, ', ',')
    # Dunno
    Dunno1 = r"[^\-\w\s]"
    clean_text = re.sub(Dunno1, ' ', clean_text)
    
    return clean_text

########################################################################################
########################################################################################

#Replace empty and whitespace with "Null"
def replaceWithNull(df):
    for col in df.columns:
        df[col].replace({"":"Null"}, inplace=True)
    return df 
########################################################################################
########################################################################################

def splitToCSV(df):

    authors = pd.DataFrame(columns=["name", "article_id"])
    keywords = pd.DataFrame(columns=["keyword", "article_id"])
    meta_keywords = pd.DataFrame(columns=["meta_keyword", "article_id"])
    tags = pd.DataFrame(columns=["tag", "article_id"])

    size = len(df)
    count_of_author_indexes = 0
    count_of_keyword_indexes = 0
    count_of_meta_keyword_indexes = 0
    count_of_tag_indexes = 0
    
    df['Authors'] = df['Authors'].astype(str)

    df["Meta_description"] = df["Meta_description"].replace(",","")
    df["Content"] = df["Content"].replace(",","")
    df["Title"] = df["Title"].replace(",","")
    df["Authors"] = df["Authors"].replace("America, There", "America There")
    df['Summary']=df['Summary'].replace(',','')
    df["Summary"] = df["Summary"].replace(",","")
    df["Meta_keywords"] = df["Meta_keywords"].replace("[", "")
    df["Meta_keywords"] = df["Meta_keywords"].replace("]", "")
    df["Meta_keywords"] = df["Meta_keywords"].replace("'", "")
    df["Meta_keywords"] = df["Meta_keywords"].replace("-\\xa0", "")
    

    for i in tqdm(range(1, size)):

        #df.loc[i,"Content"] = clean_text(df.loc[i,"Content"])
        #df.loc[i, "Meta_description"] = clean_text(df.loc[i, "Meta_description"])
        #df.loc[i, "Title"] = clean_text(df.loc[i, "Title"])
        #df.loc[i, "Meta_keywords"] = clean_text(df.loc[i, "Meta_keywords"])
        #df.loc[i, "Authors"] = clean_text(df.loc[i, "Authors"])

        if df.loc[i, "Authors"] != "Null":
            list_of_authors = df.loc[i,"Authors"].split(", ")
            num_of_authors = len(list_of_authors)

            for j in range(num_of_authors):
                
                authors.loc[count_of_author_indexes, "name"] = list_of_authors[j]
                authors.loc[count_of_author_indexes, "article_id"] = df.loc[i, "Id"]
                count_of_author_indexes += 1

        if df.loc[i, "Keywords"] != "Null":
            list_of_keywords = df.loc[i, "Keywords"].split(", ")
            num_of_keywords = len(list_of_keywords)
           
            for j in range(num_of_keywords):
                
                keywords.loc[count_of_keyword_indexes, "keyword"] = list_of_keywords[j]
                keywords.loc[count_of_keyword_indexes, "article_id"] = df.loc[i, "Id"]
                count_of_keyword_indexes += 1
        
        
        df.loc[i, "Meta_keywords"] = df.loc[i, "Meta_keywords"].replace("[", "")
        df.loc[i, "Meta_keywords"] = df.loc[i, "Meta_keywords"].replace("]", "")
        df.loc[i, "Meta_keywords"] = df.loc[i, "Meta_keywords"].replace("'", "")
        if df.loc[i, "Meta_keywords"] != "Null":
            list_of_meta_keywords = df.loc[i, "Meta_keywords"].split(", ")
            num_of_meta_keywords = len(list_of_meta_keywords)
            
            for j in range(num_of_meta_keywords):
                
                meta_keywords.loc[count_of_meta_keyword_indexes, "meta_keyword"] = list_of_meta_keywords[j]
                meta_keywords.loc[count_of_meta_keyword_indexes, "article_id"] = df.loc[i, "Id"]
                count_of_meta_keyword_indexes += 1
        
        if df.loc[i, "Tags"] != "Null":
            list_of_tags = df.loc[i, "Tags"].split(", ")
            num_of_tags = len(list_of_tags)
            
            for j in range(num_of_tags):
                tags.loc[count_of_tag_indexes, "tag"] = list_of_tags[j]
                tags.loc[count_of_tag_indexes, "tag"] = list_of_tags[j].replace(",", "")
                tags.loc[count_of_tag_indexes, "article_id"] = df.loc[i, "Id"]
                count_of_tag_indexes += 1
    
    articles = df[["Id", "Domain", "Type", "Url", "Content", "Scraped_at", "Inserted_at", "Updated_at", "Title", "Meta_description", "Summary", "Source"]]
    articles = articles.iloc[1: , :]
    filepath = Path("../Data/authors.csv")
    authors.to_csv(filepath, index=False)
    filepath = Path("../Data/keywords.csv")
    keywords.to_csv(filepath, index=False)
    filepath = Path("../Data/meta_keywords.csv")
    meta_keywords.to_csv(filepath, index=False)
    filepath = Path("../Data/tags.csv")
    tags.to_csv(filepath, index=False)
    filepath = Path("../Data/articles.csv")
    articles.to_csv(filepath, index=False) 