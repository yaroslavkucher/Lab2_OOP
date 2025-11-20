<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<title>Бібліотечний Каталог</title>

				<link rel="icon"
				      href="data:image/svg+xml,&lt;svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22&gt;&lt;text y=%22.9em%22 font-size=%2290%22&gt;📚&lt;/text&gt;&lt;/svg&gt;" />

				<style>
					body {
					background-color: #fdf5e6;
					color: #5a4a42;
					font-family: 'Georgia', 'Times New Roman', serif;
					margin: 25px;
					}

					/* Стиль для контейнера заголовка з логотипом */
					.header-container {
					text-align: center;
					border-bottom: 2px solid #d2b48c;
					padding-bottom: 20px;
					margin-bottom: 20px;
					}

					/* Стиль для картинки-логотипу */
					.logo-img {
					width: 64px;
					height: 64px;
					vertical-align: middle;
					margin-right: 15px;
					}

					h2 {
					color: #8b4513;
					display: inline-block; /* Щоб стояв поруч з картинкою */
					vertical-align: middle;
					font-family: 'Garamond', 'Times New Roman', serif;
					font-size: 2.5em;
					margin: 0;
					}

					table {
					border-collapse: collapse;
					width: 100%;
					box-shadow: 0 4px 10px rgba(0,0,0,0.1);
					border: 1px solid #d2b48c;
					}

					th, td {
					border: 1px solid #d2b48c;
					padding: 12px;
					text-align: left;
					}

					th {
					background-color: #f5deb3;
					color: #8b4513;
					font-size: 1.1em;
					}

					tr:nth-child(even) {
					background-color: #faf0e6;
					}
				</style>
			</head>
			<body>
				<div class="header-container">
					<img src="https://cdn-icons-png.flaticon.com/512/2232/2232688.png" class="logo-img" alt="Logo"/>
					<h2>Бібліотечний Каталог</h2>
				</div>

				<table>
					<tr>
						<th>№</th>
						<th>Назва</th>
						<th>Жанр</th>
						<th>Рік</th>
						<th>Автори</th>
					</tr>

					<xsl:for-each select="library/book">
						<xsl:sort select="title"/>
						<tr>
							<td>
								<xsl:value-of select="position()"/>
							</td>
							<td>
								<i>
									<xsl:value-of select="title"/>
								</i>
							</td>
							<td>
								<xsl:value-of select="@genre"/>
							</td>
							<td>
								<xsl:value-of select="year"/>
							</td>
							<td>
								<xsl:for-each select="author">
									<xsl:value-of select="."/>
									(<xsl:value-of select="@faculty"/>)
									<xsl:if test="position() != last()">, </xsl:if>
								</xsl:for-each>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>





