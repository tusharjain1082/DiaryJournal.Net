<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl x">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <!--<xsl:template match="x:Section[not(parent::x:Section)]">
    <div>
      <xsl:apply-templates select="node()"/>
    </div>
  </xsl:template>-->

  <xsl:template match="x:Section[not(parent::x:Section)]">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <div>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </div>
  </xsl:template>


  <xsl:template match="x:Section">
    <xsl:apply-templates select="node()"/>
  </xsl:template>

  <xsl:template match="x:Paragraph">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <p>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </p>
  </xsl:template>

	<xsl:template match="x:Table">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <table>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </table>
  </xsl:template>

  <xsl:template match="x:TableColumn">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@Width != ''">
        <xsl:text>width:</xsl:text>
        <xsl:value-of select="@Width" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <th>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </th>
  </xsl:template>

  <xsl:template match="x:TableRow">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <tr>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </tr>
  </xsl:template>
	
  <xsl:template match="x:TableCell">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
  	  <xsl:if test="@BorderThickness != ''">
        <xsl:text>border:</xsl:text>
        <xsl:value-of select="1" />
        <xsl:text>;</xsl:text>
      </xsl:if>
  	  <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <td>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </td>
  </xsl:template>
	
  <xsl:template match="x:Span">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <span>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </span>
  </xsl:template>


  <xsl:template match="x:Run">
    <xsl:variable name="style">
      <xsl:if test="@FontStyle='Italic'">
        <xsl:text>font-style:italic;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontWeight='Bold'">
        <xsl:text>font-weight:bold;</xsl:text>
      </xsl:if>
      <xsl:if test="contains(@TextDecorations, 'Underline')">
        <xsl:text>text-decoration:underline;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontSize != ''">
        <xsl:text>font-size:</xsl:text>
        <xsl:value-of select="@FontSize" />
        <xsl:text>pt;</xsl:text>
      </xsl:if>
      <xsl:if test="@FontFamily != ''">
        <xsl:text>font-family:</xsl:text>
        <xsl:value-of select="@FontFamily" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="concat(substring(@Foreground, 1, 1), substring(@Foreground, 4))" />
        <xsl:text>;</xsl:text>
      </xsl:if>
      <xsl:if test="@Foreground-Color != ''">
        <xsl:text>color:</xsl:text>
        <xsl:value-of select="@Foreground-Color"/>
        <xsl:text>;</xsl:text>
      </xsl:if>
    </xsl:variable>
    <span>
      <xsl:if test="normalize-space($style) != ''">
        <xsl:attribute name="style">
          <xsl:value-of select="normalize-space($style)"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="text()"/>
      <xsl:apply-templates select="node()"/>
    </span>
  </xsl:template>

</xsl:stylesheet>