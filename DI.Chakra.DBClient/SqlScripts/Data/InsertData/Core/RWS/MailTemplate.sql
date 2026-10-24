update DI_M_MailTemplate set MailSubject='IDAS - '+MailSubject where Id in ( select Id from DI_M_MailTemplate where MailSubject not like 'IDAS%')

update DI_M_MailTemplate set MailFrom='itidadmin@rwsentosa.com'
