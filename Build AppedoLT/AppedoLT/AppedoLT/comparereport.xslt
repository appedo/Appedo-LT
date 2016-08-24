<?xml version="1.0" encoding="utf-8"?>
<!-- Edited by XMLSpy® -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="//comparereport">
    <html>
      <head>
        <title>Appedo</title>
        <style>
          body {
          font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
          font-size: 14px;
          line-height: 1.42857143;
          color: #333;
          background-color: #fff;
          }

          .graphheadingrow
          {
          text-align: center;
          border-bottom: 1px solid #1b639e;
          }

          .graph
          {
          width: 400px;
          height: 250px;
          padding: 25px;
          }
        </style>
      </head>
      <body>
        <span>
          Script Name: <b>
            <xsl:value-of select="//comparereport/@script"/>
          </b>
        </span>
        <br/>
        <xsl:apply-templates select="summaryreport"/>
        <xsl:apply-templates select="requestresponse"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="summaryreport">
    <h1 style="width:600px; padding-bottom: 6px; background-color:#31c0be; color:#FFFFFF; text-align: center;">Appedo</h1>

    <h4> Summary Report</h4>
    <table border="1" cellspacing="0">
      <xsl:for-each select="attribute">
        <tr>
          <td style="bgcolor=#9acd32">
            <b>
              <xsl:value-of select="@displayName"/>
            </b>
          </td>
          <xsl:for-each select="report">
            <td style="text-align:{@align}">
              <xsl:value-of select="@val"/>
            </td>
          </xsl:for-each>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="requestresponse">
    <h4> Request response</h4>
    <table border="1" cellspacing="0" cellpadding="0">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th style="width: 300px; max-width: 300px;">Report Name</th>
        <th style="width: 80px;">Hit Count</th>
        <th style="width: 140px;">Throughput (Byte)</th>
        <th style="width: 80px;">Min(ms)</th>
        <th style="width: 80px;">Max(ms)</th>
        <th style="width: 80px;">Avg(ms)</th>
        <th style="width: 80px;">Min TTFB(ms)</th>
        <th style="width: 80px;">Max TTFB(ms)</th>
        <th style="width: 80px;">Avg TTFB(ms)</th>
      </tr>
      <xsl:for-each select="request">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
          <td>
            <xsl:value-of select="@address"/>
          </td>
          <td colspan="9" border="0" style="border: none; width: 1000px;">
            <table border="1" cellspacing="0" cellpadding="0" style="width: 1000px;">
              <xsl:for-each select="report">
                <tr>
                  <td style="width: 300px; max-width: 300px;">
                    <xsl:value-of select="@name"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="@hitcount"/>
                  </td>
                  <td style="width: 140px; text-align:right">
                    <xsl:value-of select="@throughput"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@min,'#.000')"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@max,'#.000')"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@avg,'#.000')"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@minttfb,'#.000')"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@maxttfb,'#.000')"/>
                  </td>
                  <td style="width: 80px; text-align:right">
                    <xsl:value-of select="format-number(@avgttfb,'#.000')"/>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

</xsl:stylesheet>

