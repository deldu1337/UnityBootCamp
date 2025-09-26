using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CSVReader
{
    // CSV의 각 셀을 구분하기 위한 정규식 패턴
    // 따옴표 안에 있는 콤마는 무시하고 셀을 구분하기 위함
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

    // 줄바꿈을 기준으로 한 줄씩 나누기 위한 정규식 패턴
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    // 각 셀의 앞뒤 따옴표를 제거하기 위한 문자 배열
    static char[] TRIM_CHARS = { '\"' };

    // CSV 파일을 읽어 Dictionary 리스트로 반환하는 메서드
    public static List<Dictionary<string, object>> Read(string file)
    {
        // 결과를 담을 리스트
        var list = new List<Dictionary<string, object>>();

        // Resources 폴더에서 파일 로드
        TextAsset data = Resources.Load(file) as TextAsset;

        // 파일 내용을 줄 단위로 분리
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        // 줄이 1줄 이하라면(헤더만 있거나 비어있다면) 빈 리스트 반환
        if (lines.Length <= 1) return list;

        // 첫 번째 줄은 헤더로 처리
        var header = Regex.Split(lines[0], SPLIT_RE);

        // 두 번째 줄부터 데이터 처리 시작
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            // 값이 비어있다면 건너뜀
            if (values.Length == 0 || values[0] == "") continue;

            // 한 줄 데이터를 담을 Dictionary
            var entry = new Dictionary<string, object>();

            // 헤더 수와 값 수를 비교해서 더 작은 쪽까지만 반복
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                // 앞뒤 따옴표 제거 및 백슬래시 제거
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                object finalvalue = value;
                int n;
                float f;

                // 정수 변환 시도
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                // 실수 변환 시도
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }

                // 헤더 이름을 키로, 변환된 값을 저장
                entry[header[j]] = finalvalue;
            }

            // 완성된 Dictionary를 리스트에 추가
            list.Add(entry);
        }

        // 최종적으로 Dictionary 리스트 반환
        return list;
    }
}
